# ClaimForm-Importer

## About
A C# console app that reads [CMS1500](https://www.cms.gov/Medicare/CMS-Forms/CMS-Forms/CMS-Forms-Items/CMS1188854) health insurance forms from a folder, extracts the data with Azure Cognitive Services, and sends the information to Firebase.

## Usage
- Create a `.env` file within the same folder as `ClaimForm.exe`
```env
ACS_ENDPOINT=<Your ACS Endpoint>
ACS_KEY=<Your ACS Key>
MODEL_ID=<Your ACS FormRecognizer ModelID>
FIREBASE_URL=<Your Firebase URL>
```
- Run the app, where `<source-folder>` is the folder containing your CMS1500 forms:
```
ClaimForm.exe <source-folder>
```
- There are sample forms in the `sample-data\test` folder of the release
```
ClaimForm.exe sample-data\test
```
## Planning
<img src="img/whiteboard.png">

## Training Results
<table>
  <tr>
    <td>
      <img src="img/screen1.png">
    </td>
    <td>
      <img src="img/screen2.png">
    </td>
  </tr>
 </table>


## Future Improvments
- `IDatabaseClient` is generic and should have a better implementation

- `FormHandler` should have better encapsulation
  - Environment variables should be in a constructor or passed into the `SendFormAsync` method from main.

- We need a `Field` class to handle various field types.

- Eliminate processing the second useless page in the form

- Training data should have more variance in a real-world exmaple. 
  - ie. scans, mobile scans, partially handwritten, different color ink, different fonts, etc.

- More efficient use of async methods
  - Would allow faster submission times on large folders while not exceeding API Rate limits. 
  - Would allow first results to reach Firebase while other submissions are still processing in Azure.

- Error handling for missing `.env` file or incorrect values
