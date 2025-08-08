# 🥞 Pancake's QR

> **Team Project - Computer Architecture**\
> *Computer Science BSc, Year I*\
> *Faculty of Mathematics and Informatics, University of Bucharest*

> Launch date: 11.02.2025\
> Last update: 07.08.2025

## Description 

**Pancake's QR** is a full-stack web application designed to demonstrate the step-by-step process of **QR code encryption and decryption**. It provides an interactive and educational experience for users interested in understanding how QR codes work under the hood.

### Available on web: [qr.vlaxcs.dev](https://qr.vlaxcs.dev)

### Key Features:
- **Customizable Error Correction Level**  
  Choose from Low (L), Medium (M), Quartile (Q), or High (H) to explore how QR codes handle data recovery in case of damage or distortion.

- **Adjustable QR Code Version**  
  Select versions from 1 to 40 to see how QR code size and data capacity scale with complexity.

### Development
- **[Back-end ~ API](./QR%20Code%20Generator%20Scanner/server/)**: C# (.NET 8)
- **[Front-end](./QR%20Code%20Generator%20Scanner/client/)**: React.JS with Typescript, Tailwind CSS v4.0

## Explore

1. **[QR Code terminology](#1-qr-code-terminology)**
2. **[Development prerequisites](#2-development-prerequisites)**
3. **[Back-end](#3-back-end)**
    - ***[a) API Configuration](#a-api-configuration)***
    - ***[b) Generator](#b-generator)***
    - ***[c) Scanner](#c-scanner)***
4. **[Front-end](#4-front-end)**
    - ***[a) Generator](#a-qr-code-generator)***
    - ***[b) Scanner](#b-qr-code-scanner)***
5. **[Contributors](#5-contributors)**
6. **[References](#6-references)**


## 1. QR Code terminology

| **Term**                   | **Explanation**                                                                 |
|----------------------------|----------------------------------------------------------------------------------|
| **Encoding Type**          | Specifies the type of data being encoded. Options include:<br>• **Numeric** – digits only<br>• **Alphanumeric** – digits and uppercase letters<br>• **Byte** – binary data (e.g., UTF-8)<br>• **Kanji** – Japanese characters using Shift JIS encoding |
| **QR Code Version**        | Indicates the size of the QR Code, ranging from **Version 1** (21×21 modules) to **Version 40** (177×177 modules). Higher versions support more data. |
| **Mask**                   | A value from **0 to 7** that determines the masking pattern applied to the QR Code to improve readability and reduce scanning errors. |
| **Error Correction Level** | Defines the amount of data that can be restored if the QR Code is damaged. Levels include:<br>• **Low (L)** – ~7% recovery<br>• **Medium (M)** – ~15% recovery<br>• **Quartile (Q)** – ~25% recovery<br>• **High (H)** – ~30% recovery |

- Complete explanaition could be found on wikipedia<sup>[1](#6-references)</sup>.

## 2. Development prerequisites

- **Back-end**
  - **.NET 8 SDK (C#)**
  - Compatible development environment (Recommended: **Visual Studio 2022+**)
  - NuGet package restore **enabled**

- **Front-end**
  - **Node v22.14+**
  - **npm v10.9+**
  - Compatible development environment (Recommended: **Visual Studio Code**)


## 3. Back-end

This is the **back-end** of the Pancake's QR application, built with **C# (.NET 8)**. It contains the core API logic for both QR code generation and scanning.

### a) API Configuration

  - 📁 Root Directory: `server`

  - 📄 [`Program.cs`](./QR%20Code%20Generator%20Scanner/server/)
    - Responsible for **configuring the API pipeline** and services.
    - Uses `Microsoft.AspNetCore` for building and hosting the web API.
    - Integrates `Microsoft.OpenApi` to **serialize API metadata into JSON**, enabling **Swagger UI** for interactive documentation.
    - Defines **two controllers**:
      - [`📄 Generator.cs`](./QR%20Code%20Generator%20Scanner/server/Controllers/Generator.cs) – handles QR code creation;
      - [`📄 Scanner.cs`](./QR%20Code%20Generator%20Scanner/server/Controllers/Scanner.cs) – handles QR code decoding.

  - 📦 [`QR.csproj`](./QR%20Code%20Generator%20Scanner/server/QR.csproj)
    - Specifies in-production .NET version.
    - NuGet Packages:
      ```xml
      <ItemGroup>
        <PackageReference Include="SixLabors.ImageSharp" Version="3.1.6" />
        <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
      </ItemGroup>
      ```
  - 🔧 Package Details:
    - **SixLabors.ImageSharp**: Used for image processing, including reading and writing PNG files;
    - **Swashbuckle.AspNetCore**: Enables Swagger integration for API documentation and testing.

### b) Generator

  - [`📄 Generator.cs`](./QR%20Code%20Generator%20Scanner/server/Controllers/Generator.cs)
    - This module creates the QR code by analyzing data blocks and applying error correction. Key configuration steps:
    - **Step I - Request validation**
      ```csharp
      // Request format
      public class GenerateRequest
        {
            public required string Message { get; set; }
            public int reqVersion { get; set; }
            public int reqEccLevel { get; set; }
        }

      ...
      public IActionResult Post([FromBody] GenerateRequest request)
      ...
      QRCode newCode = QRCodeGenerator.Generate(request.Message, request.reqEccLevel, request.reqVersion);
      ```
      - Method parameters:
        - `Message`: The string to encode;
        - `reqEccLevel`: Minimum error correction level (if the optimal QR code **cannot be generated**, a **suitable level will be chosen**);
        - `reqVersion`: Minimum QR version (if **not provided or invalid**, an **appropriate version will be selected automatically**).

    - **Step II - Data processsing**
      
      | Function           | Purpose                                                                 |
      |--------------------|-------------------------------------------------------------------------|
      | `PutStripes`       | Places alternating black-and-white pixels on row and column 6 of the binary matrix. These *stripes* assist readers in detecting and aligning the QR code correctly.
      | `PutBlocks`           | Maps available regions in the matrix for placing data and error blocks.  |
      | `PutAlignmentPoints`  | Inserts alignment patterns to ensure correct orientation and decoding.  |
      | `ApplyVersionBits` | If the QR version is 6 or higher, this function adds version information bits. These bits ensure proper reading of larger QR codes.
      | `SetAllDataBlocks` | Places both **data blocks** and **error correction blocks** into the QR matrix. **Data blocks store** the actual message; error correction blocks allow for recovery in case of data loss or corruption.
      | `PlaceBestMask`    | Chooses the best mask from the 8 possible options. The optimal mask ensures an even pixel distribution and minimizes potential read errors.

    - **Step III - Message encoding**
      - Based on the input message, a bitstream is built according to the selected encoding type: (**Numeric / Alphanumeric / Byte** / Kanji — *Kanji is not supported yet*)
      - Each encoding type has some particularities and there are implemented specified classes.

      | Encoding Type   | Class Used            | Characters per Chunk | Bits per Chunk | Notes                              |
      |-----------------|-----------------------|-----------------------|----------------|-------------------------------------|
      | **Numeric**         | `NumericEncoder`      | 3 digits              | 11 bits        | Most space-efficient for numbers    |
      | **Alphanumeric**    | `AlphanumericEncoder` | 2 characters          | 10 bits        | Supports uppercase letters and digits |
      | **Byte**            | `ByteEncoder`         | 1 character           | 8 bits         | Supports full UTF-8 range           |

      - Each specialized encoder returns a `QREncodedMessage` object containing:
        - Final bitstream;
        - Length metadata;
        - Ready-to-place binary data for matrix insertion.

    - **Step IV - Data Integrity**
      -  Error Correction Block Analysis: Data blocks are split into *short* and *long* groups (according to [this table](https://www.thonky.com/qr-code-tutorial/error-correction-table))<sup>[2](#6-references)</sup> and chunked before being passed to the `encode` function from the `Galois Field` class, along with the `nsym` parameter (number of error correction blocks to generate).
      - The encoding process is based on Reed–Solomon encoding/decoding algorithm, which requires Galois Field GF(256) implementation.<sup>[3](#6-references)</sup>.

    - **Step V - Output data**
      - **API**: A **data array** used to create the PNG file. | **CLI:** A **PNG file** containing the generated QR code;
      - The **version** used for encoding;
      - The **error correction level** used.

### c) Scanner

  - [`📄 Scanner.cs`](./QR%20Code%20Generator%20Scanner/server/Controllers/Scanner.cs)
    -  **Step I - Request validation**
        ```csharp
        public async Task<IActionResult> PostAsync(IFormFile image)
        ...
        QRCode code = QRCodeImageParser.Parse(tempPath);  // Described at Step II
        var decoded = QRCodeDecoder.DecodeQR(code);
        ```
        - Method parameters:
          - `Image`: A PNG file sent via the API.

    - **Step II - Binary Matrix construction**
      - The image is parsed into a binary matrix using:
        ```csharp
        var code = QRCodeImageParser.Parse(@"filepath");
        ```
      -  `Parse` Method steps:

        | Step                  | Description                                                                 |
        |-----------------------|-----------------------------------------------------------------------------|
        | `DetermineBounds`     | Crops out padding by analyzing luminance.                                    |
        | `DeterminePixelSize`  | Scans diagonals to estimate module (pixel) size.                             |
        | `ExtractJaggedArray`  | Builds the binary matrix from the image.                                     |
        | `CheckOrientation`    | Rotates matrix to align finder patterns (Northwest, Southwest, Southeast).   |

      - Once complete, the matrix is passed forward for decoding and the determined data is stored:
        - QR version;
        - Error correction level;
        - Mask pattern.

        <img src="./Documentation%20Previews/VECC.png" alt="VECC" width="300"/>

        *If vertical alignment lines mismatch, processing halts. `GetClosestDataEC` is used to resolve ambiguity.*

      -  Useful methods for easier debug:

      | Method         | Description                                      |
      |----------------|--------------------------------------------------|
      | `SaveToFile()` | Saves the matrix as a PNG image.                 |
      | `Print()`      | Outputs the QR structure to the CLI.             |

    - **Step III - Datablocks processing**
      - The decoding process is triggered using:
      ```csharp
      QRCodeDecoder.DecodeQR(code);
      ```

      - This method converts the binary matrix back into the original message, applying Reed-Solomon algorithm for decoding ECC datablocks.
      - **Error Correction Blocks**:
        - Data is split into **short** and **long** blocks.
        - Each block is decoded using `GaloisField.decode()`.

        | Encoding Type | Characters Encoded | Bits Used |
        |---------------|---------------------|-----------|
        | Numeric        | 3 digits            | 11 bits   |
        | Alphanumeric   | 2 characters        | 10 bits   |
        | Byte           | 1 character         | 8 bits    |

      - **Reed-Solomon Correction**:
        - Implemented via the `GaloisField` class.
        - Key methods:

        | Method                | Purpose                                           |
        |-----------------------|---------------------------------------------------|
        | `GeneratePolynomials` | Builds GF(256) log/antilog tables.                |
        | `Multiply` / `Divide` | Performs arithmetic in GF(256).                   |
        | `PolyMul` / `PolyAdd` | Polynomial multiplication and addition.           |
        | `PolyScale`           | Scales a polynomial by a constant.                |
        | `PolyEval`            | Evaluates a polynomial at a given `x` value.      |
        | `RSGeneratorPoly`     | Creates Reed–Solomon generator polynomial.        |
        | `Decode` / `Encode`   | Operates on UTF-8 byte arrays in 256-byte chunks. |

        - Multiplication with logs/antilogs is explained in this article from thonky.com <sup>[5](#6-references)</sup>.

    - **Step IV - Output data**
      - **Decoded message**: The original message extracted from the QR code;
      - **Version**: The QR code's version discovered in decoding process;
      - **Error Correction Level**: Discovered QR code's error correction after applying Reed-Solomon algorithm. 

## 4. Front-end

### a) QR Code Generator

- Offers a **web interface** where users can:
  - Input custom text'
  - Select a desired **Error Correction Level (ECC)** and **QR Code Version**.
- If ECC and Version are not specified, the application will **automatically determine the optimal configuration** based on the input.
- The output is a **PNG file** of the generated QR code.
- After generation, the interface displays the **selected ECC Level and Version**, giving users insight into the encoding parameters used.

### b) QR Code Scanner

- Provides a **web interface** (but also there is also available a **CLI alternative**) for uploading a **PNG image** of a plain, unencrypted QR code.
- Upon successful decoding, the **decrypted message** is displayed in a response box, with an option to **copy** the result.
- If the decoding fails, the scanner will display a **clear error message** to inform the user.


## 5. Contributors
- [Vlad MINCIUNESCU (vlaxcs)](https://github.com/vlaxcs)<br>
- [Ștefan BUSOI (stefanbusoi)](https://github.com/stefanbusoi)<br>
- [Vlăduț GHIȚĂ (Dalv-Dalv)](https://github.com/Dalv-Dalv)<br>
- [Rafel BĂLĂCEANU (1BRG)](https://github.com/1BRG)<br>


## 6. References
1. https://en.wikipedia.org/wiki/QR_code
2. https://www.thonky.com/qr-code-tutorial/error-correction-table
3. https://en.wikipedia.org/wiki/Reed%E2%80%93Solomon_error_correction#MATLAB_example
4. https://www.nayuki.io/page/creating-a-qr-code-step-by-step
5. https://www.thonky.com/qr-code-tutorial/error-correction-coding#step-6-understand-multiplication-with-logs-and-antilogs


## Bonus. Internet never disappoints...
-1. https://blog.qartis.com/decoding-small-qr-codes-by-hand/ \
-2. https://people.inf.ethz.ch/gander/papers/qrneu.pdf 
