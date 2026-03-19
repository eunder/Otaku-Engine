using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class WorkshopQueryOntoBlocks : MonoBehaviour
{

    private static WorkshopQueryOntoBlocks _instance;
    public static WorkshopQueryOntoBlocks Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    public GameObject blockPrefab;
    public GameObject blockMenu;

    public GameObject blockPrefab_UpdateMapPick;
    public GameObject blockMenu_UpdateMapPick;

    public List<GameObject> blockList = new List<GameObject>();

    //BAD, USE ANOTHER SOLUTION LATER
    bool madeByme = false;
    bool sortByVote = false;
    bool sortByDate = false;
    bool subscribed = false;
    bool favorited = false;

    bool tag_safe = false;
    bool tag_questionable = false;
    bool tag_mature = false;

    public Button backButton;

    public void StartSearchByMe()
    {
        madeByme = true;
        sortByVote = false;
        sortByDate = false;
        subscribed = false;
        favorited = false;

        pageIndex = 1;
        DoSearch(blockMenu, blockPrefab);
    }

    public void StartSearchToUpdateByMe()
    {
        madeByme = true;
        sortByVote = false;
        sortByDate = false;
        subscribed = false;
        favorited = false;

        pageIndex = 1;
        DoSearch(blockMenu_UpdateMapPick, blockPrefab_UpdateMapPick);
    }
    public void StartSearchByVote()
    {
        madeByme = false;
        sortByVote = true;
        sortByDate = false;
        subscribed = false;
        favorited = false;

        pageIndex = 1;
        DoSearch(blockMenu, blockPrefab);
    }
    public void StartSearchByDate()
    {
        madeByme = false;
        sortByVote = false;
        sortByDate = true;
        subscribed = false;
        favorited = false;

        pageIndex = 1;
        DoSearch(blockMenu, blockPrefab);
    }
    public void StartSearchSubscribed()
    {
        madeByme = false;
        sortByVote = false;
        sortByDate = false;
        subscribed = true;
        favorited = false;

        pageIndex = 1;
        DoSearch(blockMenu, blockPrefab);
    }
    public void StartSearchByFavorited()
    {
        madeByme = false;
        sortByVote = false;
        sortByDate = false;
        subscribed = false;
        favorited = true;

        pageIndex = 1;
        DoSearch(blockMenu, blockPrefab);
    }



    int pageIndex = 1;

    public void PageForward()
    {
        pageIndex++;
        DoSearch(blockMenu, blockPrefab);
    }
    public void PageBack()
    {
        pageIndex--;
        DoSearch(blockMenu, blockPrefab);
    }

    public async void DoSearch(GameObject ContentField, GameObject prefabBlock)
    {
        tag_safe = ConfigFileHandler.Instance.configData.Tags.safe;
        tag_questionable = ConfigFileHandler.Instance.configData.Tags.questionable;
        tag_mature = ConfigFileHandler.Instance.configData.Tags.mature;


         var query = Steamworks.Ugc.Query.Items;

    if(!tag_safe && !tag_questionable && !tag_mature) // BAD SOLUTION; makes sure atleast is one tag is selected so that NOTHING shows up. Originally, if not tag wasnt selected, EVERYTHING from the workshop would show up...
    {
        foreach(GameObject obj in blockList)
        {
            Destroy(obj);
        }

        blockList.Clear();
        return;
    }


    if(madeByme || favorited || subscribed) //if you dont seperate tags this way... for some reason steam wont properly filter out things...(i tried and tried but this is the final solution i can come up with)
    {
        if(madeByme)
        query = query.WhereUserPublished();

        if(favorited)
        query = query.WhereUserFavorited();

        if(subscribed)
        query = query.WhereUserSubscribed();

    }
    else
    {


        if(sortByVote)
        query = query.SortByVoteScore();

        if(sortByDate)
        query = query.RankedByPublicationDate();

        //tags
        if(tag_safe)
        query = query.WithTag("Safe");

        if(tag_questionable)
        query = query.WithTag("Questionable");

        if(tag_mature)
        query = query.WithTag("Mature");

        query = query.MatchAnyTag();

    }


     

    
        var result = await query.GetPageAsync(pageIndex);
        Debug.Log( $"ResultCount: {result?.ResultCount}" );
        Debug.Log( $"TotalCount: {result?.TotalCount}" );


        foreach(GameObject obj in blockList)
        {
            Destroy(obj);
        }

        blockList.Clear();

        if(result.HasValue)
        {
        foreach ( Steamworks.Ugc.Item entry in result.Value.Entries )
        {
            GameObject block = Instantiate(prefabBlock ,new Vector3(0,0,0), Quaternion.identity);
            block.transform.SetParent(ContentField.transform);
            block.GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().AssignBlockInfo(entry.Title, entry, entry.PreviewImageUrl, entry.Directory);
            blockList.Add(block);
        }
        }
        else
        {
            
        }

        if(pageIndex <= 1)
        {
            pageIndex = 1;
            backButton.interactable = false;
        }
        else
        {
            backButton.interactable = true;
        }

        UpdateBlockList_Icons();
    }


    public void UpdateBlockList_Icons()
    {
        foreach(GameObject block in blockList)
        {
            block.GetComponent<WorkshopQueryOntoBlocks_BlockInfo>().UpdateStatus_Icons();
        }
    }


}
