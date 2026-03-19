using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Text;

//This uses the temp operating system folder for unpacking(loading) zip file levels.
//and when saving... it uses whatever directory the original zip is in to create a "_PROGRESS" zip. this zip will replace the original zip


public class ZipFileHandler : MonoBehaviour
{
    private static ZipFileHandler _instance;
    public static ZipFileHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }


    passwordConfirm_Button.SetActive(false);
    }


    public GameObject password_Window;
    public GameObject passwordConfirm_Button;
    public TMP_InputField password_InputField;
    public Image lockImage;
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public AudioSource lockAudioSource;
    public AudioClip lockedAudioClip;
    public AudioClip unlockedAudioClip;


    public GameObject progressWindow;

    public TextMeshProUGUI progressLabel;

    public Slider progressBar;
    public Button cancelButton;


    //black screen (this is used instead of the other screen to make the game more immersive at times)
    public GameObject blackScreen_Canvas;
    public TextMeshProUGUI blackScreen_progressText;



    public async void AttemptToOpenZipFile(string path, string password = "", bool additive = false, bool wasOpenedWithMenu = true,  Transform objToParentTo = null)
    {
        Debug.Log("AttemptToOpenZIpFIle: " + path + " Additive: " + additive);

        //general file safety check
        if(!GlobalUtilityFunctions.IsSafeToReadBytes(path))
        {
            return;
        }


        //check for zip magic number
        byte[] zipByte = File.ReadAllBytes(path);

        byte[] zipMagicNumber = new byte[] { 0x50, 0x4B, 0x03, 0x04};

        int matchIndex = zipByte.AsSpan(0, 5).IndexOf(zipMagicNumber); //returns -1 if no match found
        if(matchIndex == -1)
        {
            UINotificationHandler.Instance.SpawnNotification("<color=red>File type not allowed");
            return;
        }


        //the path that will be passed to the zip file unpacker bellow
        string pathWorkingWith;

        //only only change the "currentPathWorkingWith" if it is not an additive map...
        if(additive == false)
        {
                ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = path;

                //check to see if the map is in the current opened project folder 
                if(File.Exists(ProjectManager.currentOpenedProjectPath + "/maps/" + path))
                {
                    ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = ProjectManager.currentOpenedProjectPath + "/maps/" + path;
                }
                pathWorkingWith = ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith;

        }
        else
        {
             pathWorkingWith = path;
            if(File.Exists(pathWorkingWith + "/maps/" + path))
            {
                pathWorkingWith = ProjectManager.currentOpenedProjectPath + "/maps/" + path;
            }

        }
        


        //Attempt to open zip. (if there is a file encrypted, prompt the user for a password, if not... proceed to unpack the map)
        using(ZipFile currentProjectZipFile = new ZipFile(pathWorkingWith))
        {
                foreach (ZipEntry entry in currentProjectZipFile)
                {
                    if(entry.IsCrypted)
                    {
                        PromptUserForPassword();
                        return;
                    }
                }


                //if no file is encrypted... then unpack zip file into a temporary folder

                //set the current password working with to empty... or else when you save an uncrypted map... it will get encrypted
                ZipFileHandler_GlobalStaticInfo.currentPasswordWorkingWith = "";
              
                string tempFolderName;

                //create temp folder
                string tempPath = System.IO.Path.GetTempPath();
                tempFolderName = tempPath + "TEMPORARY_EXTRACTED_ZIP_" +  Path.GetFileNameWithoutExtension(pathWorkingWith);
                System.IO.Directory.CreateDirectory(tempFolderName);
                ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory = tempFolderName;


                //validate and extract  private async Task<List<ZipEntry>> 
                List<ZipEntry> validEntries = await ValidateContentsOfZipWithProgressAsync(pathWorkingWith, tempFolderName, password, additive, wasOpenedWithMenu);
                foreach(ZipEntry e in validEntries)
                {
                    Debug.Log("ENTRY VALID: " + e.Name);
                }
                if(validEntries.Count >= 1)
                {
                await UnpackZipWithProgressAsync(pathWorkingWith, tempFolderName, validEntries, password, additive, wasOpenedWithMenu, objToParentTo);
                }
        }
    }


//Returns a string of text... to use like:        
// string json = ReadFirstJsonFromZip(path);            
// JsonUtility.FromJson<LevelData>(json)"

public static string ReadFirstJsonFromZip(string zipFilePath)
{
    // Open the ZIP file
    using (FileStream fs = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
    using (ZipFile zipFile = new ZipFile(fs))
    {
        // Iterate through all entries in the ZIP archive
        foreach (ZipEntry entry in zipFile)
        {
            // Check if the entry is a JSON file (ends with .json)
            if (entry.Name.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
            {
                if(entry.IsCrypted)
                {
                    return "ENCRYPTED";
                }
                // Open the entry stream and read the content
                using (StreamReader reader = new StreamReader(zipFile.GetInputStream(entry)))
                {
                    return reader.ReadToEnd(); // Return the content of the first JSON file
                }
            }
        }
    }

    // If no JSON file is found
    throw new FileNotFoundException("No JSON file found in the ZIP archive.");
}







    public void PromptUserForPassword()
    {

        DisablePlayer();

        passwordConfirm_Button.SetActive(true);
        password_Window.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();


        //focus on inputfield
        password_InputField.Select();
        password_InputField.ActivateInputField();
    }
    
    //used mostly just for letting the player press enter to attempt password
    void Update()
    {

        if(passwordConfirm_Button.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmPassword();
            }
        }
    }



    public async void ConfirmPassword()
    {
        //disable button until the function is finished
        passwordConfirm_Button.SetActive(false);


        //check if user guessed the password
        Task<bool> resultTask = CheckIfPasswordWasGuessedRight(password_InputField.text);
        bool result = await resultTask;

        if(result == true)
        {
            lockAudioSource.PlayOneShot(unlockedAudioClip, 1.0f);

            lockImage.sprite = unlockedSprite;
            lockImage.color = Color.green;
            lockImage.DOColor(Color.white, 0.1f).SetLoops(6, LoopType.Yoyo);


            //create temp folder
            string tempPath = System.IO.Path.GetTempPath();
            string tempFolderName = tempPath + "TEMPORARY_EXTRACTED_ZIP_" +  Path.GetFileNameWithoutExtension(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith);
            System.IO.Directory.CreateDirectory(tempFolderName);
            ZipFileHandler_GlobalStaticInfo.currentUnzippedTempDirectory = tempFolderName;



            await Task.Delay(1800);

            password_Window.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();


            //validate and extract
            List<ZipEntry> validEntries = await ValidateContentsOfZipWithProgressAsync(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith, tempFolderName, password_InputField.text);
            if(validEntries.Count >= 1)
            {
            await UnpackZipWithProgressAsync(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith, tempFolderName, validEntries, password_InputField.text);
            }
        }
        else
        {
                    lockAudioSource.PlayOneShot(lockedAudioClip, 1.0f);
                    
                    lockImage.sprite = lockedSprite;
                    lockImage.color = Color.red;
                    lockImage.DOColor(Color.green, 0.2f);
                    lockImage.rectTransform.DOShakePosition(duration: 0.2f, strength: new Vector3(6f, 6f, 0f), vibrato: 50, fadeOut: false);
               //     UINotificationHandler.Instance.SpawnNotification("<color=red> Incorrect Password" , UINotificationHandler.NotificationStateType.error);
      
                    passwordConfirm_Button.SetActive(true);

                    //focus on inputfield
                    password_InputField.Select();
                    password_InputField.ActivateInputField();

        }


    }


        //to guess if the user guessed to password right... attempt to extract json file(s) from the zip...
        private async Task<bool> CheckIfPasswordWasGuessedRight(string password)
        {
            try
            {
                            FastZip zipFile = new FastZip();
                            zipFile.Password = password;



                            string tempPath = System.IO.Path.GetTempPath();
                            string tempFolderName = tempPath + "TEMPORARY_EXTRACTED_ZIP_" +  Path.GetFileNameWithoutExtension(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith);

                            string filter = @"\.json$"; // Only files ending in ".json"
                            zipFile.ExtractZip (ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith, tempFolderName, filter);
                            ZipFileHandler_GlobalStaticInfo.currentPasswordWorkingWith = password;
                            return true;

                return false;
                
            }
            catch (ZipException ex)
            {
                ZipFileHandler_GlobalStaticInfo.currentPasswordWorkingWith = "";
                // Other exceptions may occur (e.g., invalid ZIP file)
                return false;
            }
        }



    //ZIP MAGIC NUMBER CHECKING MECHANIC (NEEDS ITS OWN LOOP BECAUSE SHARPZIPLIB SKIPS ENTRIES EVEN IF PARTIALLY READ!)
    private async Task<List<ZipEntry>> ValidateContentsOfZipWithProgressAsync(string zipPath, string extractPath, string password, bool additive = false, bool wasOpenedWithMenu = true)
    {
        List<ZipEntry> validEntries = new List<ZipEntry>();


        progressLabel.text = "Checking zip file...";

        blackScreen_progressText.gameObject.SetActive(true);

        if(wasOpenedWithMenu)
        {
            progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
        }
        else
        {
            if(additive == false)
            {
                blackScreen_Canvas.SetActive(true);

                if(SimpleSmoothMouseLook.Instance)
                {
                    SimpleSmoothMouseLook.Instance.enabled = false;
                }
                if(PlayerMovementBasic.Instance)
                {  
                    PlayerMovementBasic.Instance.enabled = false;
                }
                if(WheelPickerHandler.Instance)
                {  
                    WheelPickerHandler.Instance.wheelPickerIsOpen = true;
                }
            }
        }

        //so that the player can see the window even open 
        await Task.Delay(150);


        using (FileStream fsIn = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
       
       
    try
    {
                    //used to catch exceptions in that "await Task.Run ->" area
                    string asyncExceptionMessage = ""; 

        using (ZipInputStream zipIn = new ZipInputStream(fsIn))
        {

            zipIn.Password = password;

            ZipEntry entry;
            while ((entry = zipIn.GetNextEntry()) != null)
            {
                // Check if cancellation is requested
                if (canceled)
                {
                    ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = "";
                    CloseGUIStuff();
                    UINotificationHandler.Instance.SpawnNotification("<color=red>ZipVld: " + asyncExceptionMessage , UINotificationHandler.NotificationStateType.error);
                    return validEntries;
                }

                // Get the current entry name
                string entryName = entry.Name;

                // Configure the extraction process
                entry.IsUnicodeText = true;



                //if its a json... validate it in a different way... because jsons dont have magic numbers...
                if(entry.Name.EndsWith(".json"))
                {
                    const long maxSize = 25 * 1024 * 1024;
                    if(entry.Size < maxSize)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            zipIn.CopyTo(ms); 
                            string json = Encoding.UTF8.GetString(ms.ToArray());

                            if(GlobalUtilityFunctions.IsJsonFileSafe(json))
                            {
                                validEntries.Add(entry);
                            }

                        }
                    }
                    else
                    {
                            UINotificationHandler.Instance.SpawnNotification("<color=red>map file too large!");
                    }
                }
                else
                {
                    //VALIDATE THE "MAGIC NUMBERS" OF THE FILE
                    // Read the first few bytes of the entry
                        byte[] buffer = new byte[512];
                        int bytesRead = zipIn.Read(buffer, 0, 512);
                        Debug.Log("Buffer: " + BitConverter.ToString(buffer));
                        
                        if(GlobalUtilityFunctions.IsFileDataSafe(buffer))
                        {
                                validEntries.Add(entry);
                        }
                        else
                        {
                                UINotificationHandler.Instance.SpawnNotification("<color=red>ZipVld File: " + entry.Name);
                        }
                }


                // Update the progress bar based on the current entry
                float progress = (float)(zipIn.Position) / fsIn.Length;

                if (progressBar != null && progressBar.gameObject.activeSelf)
                {
                    progressBar.value = progress;
                }

                //update the black screen text info thing too
                blackScreen_progressText.text = "Checking zip file... ";


                // Yield control back to the Unity main thread
                await Task.Yield();         
               }

        }
    

        // Check if cancellation is requested
        if (canceled)
        {
            ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = "";
            CloseGUIStuff();
            UINotificationHandler.Instance.SpawnNotification("<color=red>ZipVld: " + asyncExceptionMessage , UINotificationHandler.NotificationStateType.error);
        }

        // Reset the progress bar when done
        if (progressBar != null)
        {
            progressBar.value = 0f;
        }


        //reset cancel bool event
        canceled = false;

    }
        catch (ZipException ex)
    {
        ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = "";
        CloseGUIStuff();
        canceled = false;
        progressBar.value = 0f;
        progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
        UINotificationHandler.Instance.SpawnNotification("<color=red>ZipVld: " + ex.Message , UINotificationHandler.NotificationStateType.error);
    }

        return validEntries;
    }








    //ASYNC EXTRANCTION MECHANIC

    private bool canceled = false;


    public void CancelFileUnpackingOrPacking()
    {
        // Set the cancellation flag to true
        canceled = true;
    }


    // "wasOpenedWithMenu" is used to make sure to only use that unpacking window with manually opened maps (like password protected ones, or through the tool bar)
     private async Task UnpackZipWithProgressAsync(string zipPath, string extractPath, List<ZipEntry> validEntries, string password, bool additive = false, bool wasOpenedWithMenu = true, Transform objToParentTo = null)
    {
        progressLabel.text = "Unpacking Zip File...";

        blackScreen_progressText.gameObject.SetActive(true);

        if(wasOpenedWithMenu)
        {
            progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();
        }
        else
        {
            if(additive == false)
            {
                blackScreen_Canvas.SetActive(true);

                if(SimpleSmoothMouseLook.Instance)
                {
                    SimpleSmoothMouseLook.Instance.enabled = false;
                }
                if(PlayerMovementBasic.Instance)
                {  
                    PlayerMovementBasic.Instance.enabled = false;
                }
                if(WheelPickerHandler.Instance)
                {  
                    WheelPickerHandler.Instance.wheelPickerIsOpen = true;
                }
            }
        }

        //so that the player can see the window even open 
        await Task.Delay(150);


        using (FileStream fsIn = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
       
       
    try
    {
                    //used to catch exceptions in that "await Task.Run ->" area
                    string asyncExceptionMessage = ""; 


        // Define a maximum extraction size (e.g., 100 MB)
        const long MaxExtractSize = 1L * 1024 * 1024 * 1024; //1gb
        long totalExtractedSize = 0;

        int maxFiles = 1000;
        int fileCount = 0;


        using (ZipInputStream zipIn = new ZipInputStream(fsIn))
        {

            zipIn.Password = password;

            ZipEntry entry;
            while ((entry = zipIn.GetNextEntry()) != null)
            {
                if(validEntries.Any(e=> e.Name == entry.Name))
                {
                    // Check if cancellation is requested
                    if (canceled)
                    {
                        ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = "";
                        CloseGUIStuff();
                        UINotificationHandler.Instance.SpawnNotification("<color=red>ZipUpk: " + asyncExceptionMessage , UINotificationHandler.NotificationStateType.error);
                        return;
                    }

                    // Get the current entry name
                    string entryName = entry.Name;

                    // Configure the extraction process
                    entry.IsUnicodeText = true;


                        // Skip entries containing "../" or starting with "/" (to prevent traversal or absolute paths)
                        if (entryName.Contains("..") || entryName.StartsWith("/") || entryName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                        {
                            UINotificationHandler.Instance.SpawnNotification("<color=red>Skipping potentially unsafe file" + entryName);
                            continue;
                        }

                    // Perform the extraction asynchronously
                    await Task.Run(() =>
                    {
                        try
                        {
                            string outputPath = Path.Combine(extractPath, entryName);
                            string fullOutputPath = Path.GetFullPath(outputPath);


                            //EXTREMELLY IMPORTANT FOR SECURITY!!! zip slip)
                            // Ensure the full path starts with the extraction directory 
                            if (!fullOutputPath.StartsWith(Path.GetFullPath(extractPath), StringComparison.Ordinal))
                            {
                                canceled = true;
                                throw new UnauthorizedAccessException($"Detected a Zip Slip attempt: {entryName}");
                            }




                            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));


                            // Enforce file limits
                            fileCount++;
                            if (fileCount > maxFiles)
                            {
                                throw new InvalidOperationException("Too many files in the archive.");
                            }


                            int maxDepth = 1; // Allow only files in the root of the extraction directory
                            string[] pathSegments = entryName.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                            if (pathSegments.Length > maxDepth)
                            {
                                throw new InvalidOperationException($"File {entryName} exceeds the maximum directory depth of {maxDepth}.");
                            }


                            // Check the size of the current entry
                            if (entry.Size > MaxExtractSize)
                            {
                                throw new InvalidOperationException($"File {entryName} exceeds the maximum allowed size of {MaxExtractSize} bytes.");
                            }

                            // Check cumulative size
                            totalExtractedSize += entry.Size;
                            if (totalExtractedSize > MaxExtractSize)
                            {
                                throw new InvalidOperationException("Total extracted size exceeds the allowed limit.");
                            }




                            using (FileStream fsOut = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
                            {
                                byte[] buffer = new byte[4096];
                                StreamUtils.Copy(zipIn, fsOut, buffer);
                            }
                        }
                        catch(Exception ex)
                        {
                            asyncExceptionMessage = ex.Message;
                            canceled = true;
                        }
                    });

                    // Update the progress bar based on the current entry
                    float progress = (float)(zipIn.Position) / fsIn.Length;

                    if (progressBar != null && progressBar.gameObject.activeSelf)
                    {
                        progressBar.value = progress;
                    }

                    //update the black screen text info thing too
                    blackScreen_progressText.text = "Unpacking zip file... ";




                    // Yield control back to the Unity main thread
                    await Task.Yield();         
                }
            }
        }
    

        // Check if cancellation is requested
        if (canceled)
        {
            ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = "";
            CloseGUIStuff();
            UINotificationHandler.Instance.SpawnNotification("<color=red>ZipUpk: " + asyncExceptionMessage , UINotificationHandler.NotificationStateType.error);
        
        }

        // Reset the progress bar when done
        if (progressBar != null)
        {
            progressBar.value = 0f;
        }




        if(canceled == false)
        {
        //finally...load the map
        string[] files = Directory.GetFiles(extractPath, "*.json");

        if(additive)
        {
            blackScreen_progressText.gameObject.SetActive(false);

            string json = File.ReadAllText(files[0]);
            SaveAndLoadLevel.LevelData fileLevelData;
            fileLevelData = JsonUtility.FromJson<SaveAndLoadLevel.LevelData>(json);

                    Debug.Log("Loading path ZIP ADDITIVE: " + files[0]);
                    Debug.Log("Loading path ZIP ADDITIVE EX. PATH: " + extractPath);

            StartCoroutine(SaveAndLoadLevel.Instance.LoadEntities(fileLevelData, 1, files[0], objToParentTo));

        }
        else
        {
            GlobalStaticLevelToLoad.LevelFilePathToLoadOnStartUp = files[0];
            SceneManager.LoadScene("PlayerRoom", LoadSceneMode.Single);
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
        }

        //reset cancel bool event
        canceled = false;



    }
        catch (ZipException ex)
    {
        ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith = "";
        CloseGUIStuff();
        canceled = false;
        progressBar.value = 0f;
        progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
        UINotificationHandler.Instance.SpawnNotification("<color=red>ZipUpk: " + ex.Message , UINotificationHandler.NotificationStateType.error);
    }


    }


    void CloseGUIStuff()
    {
        //close progress window
        if(progressWindow.activeSelf || canceled)
        {
            progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();
        }

        blackScreen_Canvas.SetActive(false);
        blackScreen_progressText.gameObject.SetActive(false);
    }



    //ASYNC ZIP PACKING MECHANIC

    
                                                                                                //"fileNames"... used for a safety check...
  public async Task PackZipWithProgressAsync(string[] filesToPack, string zipPath, string password, List<string> fileNames)
{
    progressLabel.text = "Packing Zip File...";

    progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().OpenWindow();

    //path of the temporary zip file "_PROGRESS"

    string path =  Path.Combine(Path.GetDirectoryName(zipPath),  Path.GetFileNameWithoutExtension(zipPath) + "_PROGRESS.zip");

    // Define expected directory (the directory where the files to pack are located)
    string expectedDirectory = Path.GetDirectoryName(zipPath); 


    await Task.Delay(150);
    string safeBaseDirectory = zipPath; 



    using (FileStream fsOut = new FileStream(zipPath, FileMode.Create , FileAccess.Write))
    {
        using (ZipOutputStream zipOut = new ZipOutputStream(fsOut))
        {
            zipOut.Password = password;

            foreach (string filePath in filesToPack)
            {
                              if(File.Exists(filePath))
                        {

                try
                {


                // Check if cancellation is requested
                if (canceled)
                {
                    break;
                }


                foreach(string s in fileNames)
                {
                    Debug.Log("Filename: " + s);
                }


                //Do not pack any GUIDs (reference posters)
                bool isValidGuid = Guid.TryParse(Path.GetFileName(filePath), out _);
                if(isValidGuid)
                {
                    //skip the entry
                    continue;
                }


                // Create a new entry in the zip file
                ZipEntry entry = new ZipEntry(Path.GetFileName(filePath));

                // Add the entry to the zip file
                zipOut.PutNextEntry(entry);

                // Perform the packing asynchronously
                await Task.Run(() =>
                {
                    try
                    {
                        //A MEDIOCRE check used to make sure only file names that were in the level end up being packed 
                        if(!fileNames.Contains(entry.Name))
                        {
                        throw new UnauthorizedAccessException("Attempted to pack unknown media path");
                        }

                        using (FileStream fsIn = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[4096];
                            StreamUtils.Copy(fsIn, zipOut, buffer);
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.Log("ERROR: " + ex + ". EntryName: " + entry.Name );
                    }
                });

                // Update the progress bar based on the current entry
                float progress = (float)(fsOut.Position) / fsOut.Length;

                if (progressBar != null)
                {
                    progressBar.value = progress;
                }

                // Yield control back to the Unity main thread
                await Task.Yield();
                }
                 catch (Exception ex)
                {
                    UINotificationHandler.Instance.SpawnNotification("<color=red>Error packing file: " + filePath + "  " + ex.Message);
                    continue;
                }

                
            }
            else
            {
                UINotificationHandler.Instance.SpawnNotification("<color=red>File" + filePath + "not found, skipping.");
                continue;
            }
        }
        }
    }

    // Reset the progress bar when done
    if (progressBar != null)
    {
        progressBar.value = 0f;
    }




    //VALIDATE CONTENTS OF ZIP! (to make sure player dosnt accidentally upload stuff they didnt mean too!!)









    UINotificationHandler.Instance.SpawnNotification("<color=green>Map Zipped!", UINotificationHandler.NotificationStateType.saved);
    UINotificationHandler.Instance.SpawnNotification("Please double check the contents of your Zip... just in case");
    progressWindow.GetComponent<ToolBarAnimation_WindowOpenAndClose>().CloseWindow();

    if(canceled == false)
    {
        //replace the original zip with the newly generated zip (only if these two path names dont match.) This is important to do or else the previously opened zip map will be overwritten!!!
        if(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith != zipPath)
        {
        File.Replace( ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith, zipPath, null, true);
        }
    }
    else
    {
        File.Delete(ZipFileHandler_GlobalStaticInfo.currentPathWorkingWith);
    }

    //reset cancel bool event
    canceled = false;

}


    //delete unzipped files inside of the temporary folder on quit
    //these are deleted by detecting anyfiles inside of the temporary folder with the string "TEMPORARY_EXTRACTED_ZIP_"
    private void OnApplicationQuit()
    {
        DeleteTempDirectories();
    }


    void DeleteTempDirectories()
    {
            string tempPath = System.IO.Path.GetTempPath();
            string[] allFolders = Directory.GetDirectories(tempPath);

            // Use LINQ to filter folders containing the search string
            string[] filteredFolders = allFolders.Where(folder => folder.Contains("TEMPORARY_EXTRACTED_ZIP_")).ToArray();
            
            foreach(string folder in filteredFolders)
            {
                Directory.Delete(folder, true);
            }

    }


    public void DisablePlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        if(SimpleSmoothMouseLook.Instance)
        {
                 SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = true;
        }
        if(PlayerObjectInteractionStateMachine.Instance)
        {
            PlayerObjectInteractionStateMachine.Instance.enabled = false;
        }
        if(PlayerMovementBasic.Instance)
        {
            PlayerMovementBasic.Instance.enabled = false;
        }
        if(PlayerMovementNoClip.Instance)
        {
            PlayerMovementNoClip.Instance.enabled = false;
        }
    }

    public void EnablePlayer()
    {
        if(EscapeToggleToolBar.toolBarisOpened == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
        if(SimpleSmoothMouseLook.Instance)
        {
            SimpleSmoothMouseLook.Instance.wheelPickerIsTurnedOn = false;
        }
        if(PlayerMovementBasic.Instance)
        {
            PlayerMovementBasic.Instance.enabled = true;
        }

        if(PlayerMovementTypeKeySwitcher.Instance)
        {
            PlayerMovementTypeKeySwitcher.Instance.EnableMovementBasedOnCurrentMovement();
        }
        }
    }





}


