# **QR-Code-Generator-Scanner**
Team project ~ CSA ~ Year I / Informatics Bachelor's Degree / Faculty of Mathematics and Informatics - University of Bucharest

# **Informații generale**

Această aplicație pune la dispoziție o soluție completă pentru encodarea și decodarea codurilor QR.

- **Backend**: C#
- **Frontend**: [Website](https://vlaxcs.github.io/QR-Code-Generator-Scanner/) - HTML, CSS, JavaScript

## **Specificațiile unui cod QR**

| Informații auxiliare     |   Specificații                            |
|--------------------------|-------------------------------------------|  
| **Tipul encodării**      | **Numeric / Alphanumeric / Byte / Kanji**  |  
| **Versiunea codului QR** | Un număr între 1 și 40, în funcție de dimensiunea datelor |  
| **Masca aplicată**       | Un număr de la 0 la 7, specific unui pattern anume |  

# 🤳 **Scanner**

### Interfața pentru utilizator

Disponibilă la adresa: https://vlaxcs.github.io/QR-Code-Generator-Scanner
![alt text](./README%20Resources/Scanner.gif)

### Date de intrare

- **Un fișier PNG** care conține un cod QR

### Prelucrarea datelor
Pasul I. <i>Crearea unei matrice binare pe baza imaginii primite (și alocarea acesteia prin constructor)</i>

```
var code = QRCodeImageParser.Parse(@"filepath");
```

- Clasa `QRCodeImageParser` conține metoda `Parse`, în care:
    - `DetermineBounds` elimină surplusul de informație din imagine, astfel încât singura informație prelucrată să fie exclusiv codul QR. Acest procecedeu se datorează diferențelor de luminanță din imagine.
    - `DeterminePixelSize` calculează dimensiunea unui pixel, prin două parcurgeri, una pe diagonala principală, iar cealaltă pe diagonala principală. La final, cele două rezultate sunt aproximate și se obține dimensiunea corectă a unui pixel.
    - `ExtractJaggedArray` calculează matricea binară care conține toate datele din codul QR. Aceasta este prelucrată ulterior pentru a stabili specificațiile și pentru a obține mesajul final.
    - `CheckOrientation` verifică orientarea codului QR. Dacă acesta nu este în poziția favorabilă prelucrării, este întors. Orientarea este determinată de poziția celor 3 pătrate din colțuri. Ne dorim ca ele să fie în Nord-Vest, Sud-Vest și Sud-Est.
    - <i>Matricea binară este trimisă la prelucrare ulterior acestor procese</i>.

- Constructorul `QRCode` creează un obiect.
    - Se  cunoaște versiunea (codului QR), determinată de dimensiunile matricii binare.

    - `Initialize` aplică măști pentru a determina informații despre codul QR (versiunea și nivelul de Erorr Correction), dispuse precum în imagine: ![alt text](./README%20Resources/VECC.png) <br> Cele două linii trebuie să fie identice. În cazul în care acestea nu sunt, programul se va opri. `GetClosestDataEC` două apeluri ale acestei funcții intermediază potențialul conflict de date.

    - `SaveToFile` transformă matricea binară într-un fișier care poate fi salvat în format PNG.

    - `Print` oferă o modalitate de previzualizare a progresului. Poate genera imaginea codului QR în CLI.

- După ce obiectul QRCode este creat, poate fi decodat cu prin apelul `DecodeQR` din biblioteca `QRCodeDecoder`.

### 🧙 Corectitudinea datelor

- Blocurile de error correction
    - Se grupează blocurile de data în short și long blocuri (conform [acestei tabele](https://www.thonky.com/qr-code-tutorial/error-correction-table)) pentru a sparge mesajul în chunk-uri care să fie trimise către funcția `decode` din clasa `Galois Field`, alături de parametrul `nsym` - numărul de blocuri de erorr correction care trebuie generate.

    - Fiecare funcție respectă formatul impus de tipul de encodare, fragmentând șirul de biți astfel:
        - Numeric: Encodează câte 3 cifre pe 11 biți
        - Alphanumeric: Encodează câte 2 caractere pe 10 biți
        - Byte: Encodează câte un caracter pe 8 biți

    Pentru fiecare tip de encodare, așa cum am menționat la început, este necesară câte o abordare diferită. Modalitățile de decodare diferă pentru o grupare de două caractere `Alphanumerice` față de o encodare precisă pentru encodarea `Byte`.

- Reed Solomon <sup>[1](#Referințe)</sup>
    - Clasa `Galois Field` conține metode utile în prelucrarea matematică a decodării/encodării cu algoritmul lui Reed-Solomon.
        - `GeneratePolynomials`. generează valorile log/antilog<sup>[4](#Referințe)</sup> în Galois Field(256).
        - `Multiply` și `Divide` înmulțește, respectiv împarte două numere cu log/antilog. <br>
        ![alt text](./README%20Resources/log.png)
        - `PolyMul` și `PolyAdd` înmulțește, respectiv adună două polinoame (tradițional).
        - `PolyScale` scalează un polinom cu un număr natural.
        - `PolyEval` evaluează valoarea unui polinom pentru un x dat. Pentru a corespunde restricțiilor Galois Field(256), termenii se adună cu operandul `XOR`.
        - `RSGeneratorPoly` generează un polinom Reed Solomon, în funcție de numărul de blocuri de error correction specificat (`nsym`).

        - `Decode` și `Encode` folosesesc un byte array (UTF-8). Șirul este ulterior decodat porționat, în chunk-uri de câte 256.

### Date de ieșire
- Mesajul decodat din codul QR primit
- Versiunea encodării
- Nivelul de corectare a erorilor

<br>

# 🖌️ **Generator**

![alt text](./README%20Resources/Generator.gif)

### Date de intrare

- **Mesajul** de encodat
- **Versiunea minimă a codului QR** (dacă nu este furnizat sau dacă este imposibil, se stabilește una corespunzătoare)
- **Nivel de corectare a erorilor** (dacă nu se poate genera un cod QR optim, se alege un nivel de corectare corespunzător) 

### Prelucrarea datelor
- Funcția `Generate` generează codul QR, analizând blocurile de date și corectând erorile. 
Configurarea versiunii și nivelului de corectare a erorilor:
    - `PutStripes` Plasează pixeli alternând între alb și negru pe linia și coloana 6 din matricea binară. Aceste "stripes" ajută la detectarea și alinierea corectă a codului QR de către cititoare.
    
    - `PutMaskBits` Aplică masca QR, care optimizează distribuția pixelilor pentru a preveni interferențele la citirea codului QR. Masca este aplicată pe blocurile de orientare pentru a spori acuratețea codului.

    - `SetAllDataBlocks` Plasează blocurile de date și blocurile de corectare a erorilor în matricea QR.
Blocurile de date conțin informațiile reale, iar blocurile de corectare a erorilor ajută la recuperarea acestora în cazul unor erori de citire.
    
    - `ApplyVersionBits` Dacă versiunea codului QR este 6 sau mai mare, această funcție adaugă biți de informații pentru versiune.
Acești biți sunt necesari pentru a asigura corectitudinea citirii pentru versiunile mai mari ale codului QR.

    - `PlaceBestMask` Alege cea mai bună mască dintre cele 8 opțiuni posibile.
Masca optimă asigură o distribuție uniformă a pixelilor, minimizând erorile de citire.

### **Message Encoder**

În funcție de mesajul primit, se consturiește un șir de biți care corespunde tipului de encodare ales (numeric/alphanumeric/byte/kanji):<br>
_Nu avem suport pentru Kanji_<br>

În clasele `NumericEncoder`, `AlphanumericEncoder`, `ByteEncoder` există metode care prelucrează șirul nou format.

Fiecare funcție respectă formatul impus de tipul de encodare, fragmentând șirul de biți astfel:
- Numeric: Encodează câte 3 cifre pe 11 biți
- Alphanumeric: Encodează câte 2 caractere pe 10 biți
- Byte: Encodează câte un caracter pe 8 biți

Toate aceste funcții de encode furnizează un `QREncodedMessage`, care conține șirul de biți care constituie mesajul final.

### Corectitudinea datelor

- Analizarea blocurilor pentru corectarea erorilor

Se grupează blocurile de data în short și long blocuri (conform [acestei tabele](https://www.thonky.com/qr-code-tutorial/error-correction-table)) pentru a sparge mesajul în chunk-uri care să fie trimise către funcția `decode` din clasa `Galois Field`, alături de parametrul `nsym` - numărul de blocuri de erorr correction care trebuie generate.

- Reed Solomon <sup>[1](#Referințe)</sup>. 

<i>Este relfecția procedeului prezentat la scanare.</i>

### Date de ieșire
- Un fișier PNG care conține codul QR generat
- Versiune folosită
- Nivelul de error correction folosit (0 - Low, 1 - Medium, 2 - Quartile, 3 - High)

## Colaboratori
[Vlad MINCIUNESCU](https://github.com/vlaxcs)<br>
[Ștefan BUSOI](https://github.com/stefanbusoi)<br>
[Vlăduț GHIȚĂ](https://github.com/Dalv-Dalv)<br>
[Rafel BĂLĂCEANU](https://github.com/1BRG)<br>

## Referințe
0. https://en.wikipedia.org/wiki/QR_code
1. https://en.wikipedia.org/wiki/Reed%E2%80%93Solomon_error_correction#MATLAB_example
2. https://www.thonky.com/qr-code-tutorial/error-correction-table
3. https://www.nayuki.io/page/creating-a-qr-code-step-by-step
4. https://www.thonky.com/qr-code-tutorial/error-correction-coding#step-6-understand-multiplication-with-logs-and-antilogs

## Internet never disappoints
-1. https://blog.qartis.com/decoding-small-qr-codes-by-hand/
-2. https://people.inf.ethz.ch/gander/papers/qrneu.pdf 
