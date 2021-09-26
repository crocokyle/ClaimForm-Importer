# ClaimForm-Importer

## About
A C# console app that reads CMS1500 health insurance forms from a folder, extracts the data with Azure Cognitive Services, and sends the information to Firebase.

## Usage
- Create a `.env` file within the same folder as `ClaimForm.exe`
```env
ACS_ENDPOINT=<Your ACS Endpoint>
ACS_KEY=<Your ACS Key>
MODEL_ID=<Your ACS FormRecognizer ModelID>
FIREBASE_KEY=<Your Firebase Key>
```
- Run the app, where `<source-folder>` is the folder containing your CMS1500 forms:
```
ClaimForm.exe <source-folder>
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


## Potential Improvments
- Training data should have more variance in a real-world exmaple. 
  - ie. scans, mobile scans, partially handwritten, different color ink, different fonts, etc.
- ACS Output should be validated and formatted in standardized ways
- Eliminate processing the second useless page in the form
- Convert `FormRecognizer.MakeRequest()` to an async function with timer.
  - Allows faster submission times on large folders while not exceeding API Rate limits. 
  - Allows early results to reach Firebase while later submissions are still processing in Azure.
