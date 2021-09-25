# ClaimForm-Importer

## About
A C# console app that reads CMS1500 health insurance forms from a folder, extracts the data with Azure Cognitive Services, and sends the information to Firebase.

## Usage
- Generate a `.env` file containing your Azure and Firebase API keys:
```env
ACS_KEY=<Your ACS Key>
FIREBASE_KEY=<Your Firebase Key>
```
- Run the app:
`ClaimForm.exe <source-folder>`

## Planning
<img src="img/whiteboard.png">

## Training Results
<img src="img/screen1.png">
<img src="img/screen2.png">


## Potential Improvments
- Training data should have more variance in a real-world exmaple. 
  - ie. scans, mobile scans, partially handwritten, different color ink, different fonts, etc.
- ACS Output should be validated and formatted in standardized ways
