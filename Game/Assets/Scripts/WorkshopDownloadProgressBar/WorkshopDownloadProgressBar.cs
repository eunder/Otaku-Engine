using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class WorkshopDownloadProgressBar : MonoBehaviour
{

    private static WorkshopDownloadProgressBar _instance;
    public static WorkshopDownloadProgressBar Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public Image fillable_Image;

    public TextMeshProUGUI downloadTitle_Text;
    public TextMeshProUGUI downloadName_Text;

    public Canvas loadbar_Canvas;

    Steamworks.Ugc.ResultPage? resultPage;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        DoSearch(downloadName_Text, loadbar_Canvas);


        Steamworks.SteamUGC.OnDownloadItemResult += OnDownloaded;

        Steamworks.SteamUserStats.OnAchievementProgress += AchievementChanged;
    }

    private void OnDownloaded(Steamworks.Result result)
    {
                    if(result == Steamworks.Result.OK)
                    GameObject.Find("CANVAS_NOTIFICATIONSUI").GetComponent<UINotificationHandler>().SpawnNotification("<color=green> Map downloaded!", UINotificationHandler.NotificationStateType.uploadedmap);
                  

                    //on object downloaded, update the current screen of blocks in the browser. Also, anything after this functions dosnt seem to get called...
                    WorkshopQueryOntoBlocks.Instance.UpdateBlockList_Icons();

    }

    private void AchievementChanged( Steamworks.Data.Achievement ach, int currentProgress, int progress )
    {
        if ( ach.State )
        {
            Debug.Log( $"{ach.Name} WAS UNLOCKED!" );   
        }
    }

    public void RefreshQueBar()
    {
        DoSearch(downloadName_Text, loadbar_Canvas);
    }




    public bool downloadingItemFound = false;

    void Update()
    {
            if(resultPage.HasValue)
            {
            
                downloadingItemFound = false;
        
                foreach ( Steamworks.Ugc.Item entry in resultPage.Value.Entries )
                {
                    if(entry.IsDownloading)
                    {
                        downloadingItemFound = true;
                        loadbar_Canvas.enabled = true;
                        fillable_Image.fillAmount = entry.DownloadAmount;
                        downloadName_Text.text = entry.Title;
                        break;
                    }
                }

                        if(!downloadingItemFound)
                        {
                            loadbar_Canvas.enabled = false;
                        }

             }
   
   
    }


public async Task DoSearch(TextMeshProUGUI downloadName_Text, Canvas loadbar_Canvas)
    {

        
        var query = Steamworks.Ugc.Query.Items;

        query = query.WhereUserSubscribed();
        query = query.SortBySubscriptionDate();
        query = query.MatchAnyTag();
        
        
        resultPage = await query.GetPageAsync(1);
        Debug.Log( $"ResultCount: {resultPage?.ResultCount}" );
        
    }
}
