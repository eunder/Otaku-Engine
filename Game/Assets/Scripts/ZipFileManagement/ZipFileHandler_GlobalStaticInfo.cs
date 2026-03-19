public static class ZipFileHandler_GlobalStaticInfo
{
       //this is used to know what the current zip file "working with" is. (this is used for "saving" zip files already opened)
       //contains ".zip" at the end
       public static string currentPathWorkingWith; 
       
       public static string currentPasswordWorkingWith;

       //the path of the zip "myMap.zip"
       public static string pathToSaveZipAs;
       public static string currentUnzippedTempDirectory;

}