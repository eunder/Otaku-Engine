using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSceneAchievementChecker : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);


    Invoke("MostLikedPublishedMapSearch", 3);
    Invoke("SubscribedMapCount", 3);
    }

    public async void MostLikedPublishedMapSearch()
    {
         var query = Steamworks.Ugc.Query.Items;
         query = query.WhereUserPublished();
         query = query.RankedByVotesUp();


        var result = await query.GetPageAsync(1);
        Debug.Log( $"ResultCount: {result?.ResultCount}" );
        Debug.Log( $"TotalCount: {result?.TotalCount}" );

        int mostLikedMap = 0;


        if(result.HasValue)
        {
        foreach ( Steamworks.Ugc.Item entry in result.Value.Entries )
        {
            if(entry.VotesUp > mostLikedMap)
            {
            mostLikedMap = (int)entry.VotesUp;
            }
            
            Debug.Log("Item Vote Count: " + entry.VotesUp + "   " + "Item Name: " + entry.Title);
        }
        }
            
            var ach_like_1 = new Steamworks.Data.Achievement("ACH_LIKE_1");
            var ach_like_10 = new Steamworks.Data.Achievement("ACH_LIKE_10");
            var ach_like_100 = new Steamworks.Data.Achievement("ACH_LIKE_100");

            if(ach_like_1.State == false)
            {
            Steamworks.SteamUserStats.SetStat("map_votesup", mostLikedMap);
            Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_LIKE_1", Steamworks.SteamUserStats.GetStatInt("map_votesup"), 2);
            }
            else if(ach_like_10.State == false)
            {
            Steamworks.SteamUserStats.SetStat("map_votesup", mostLikedMap);
            Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_LIKE_10", Steamworks.SteamUserStats.GetStatInt("map_votesup"), 10);
            }
            else if(ach_like_100.State == false)
            {
            Steamworks.SteamUserStats.SetStat("map_votesup", mostLikedMap);
            Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_LIKE_100", Steamworks.SteamUserStats.GetStatInt("map_votesup"), 100);
            }

    }

        public async void SubscribedMapCount()
    {
         var query = Steamworks.Ugc.Query.Items;
         query = query.WhereUserSubscribed();


        var result = await query.GetPageAsync(1);
        Debug.Log( $"TotalCount: {result?.TotalCount}" );

        int totalSubscribedItemsCount = (int)result?.TotalCount;
    

            var ach_subscribe_10 = new Steamworks.Data.Achievement("ACH_SUBSCRIBE_10");
            var ach_subscribe_100 = new Steamworks.Data.Achievement("ACH_SUBSCRIBE_100");
            var ach_subscribe_1000 = new Steamworks.Data.Achievement("ACH_SUBSCRIBE_1000");

            if(ach_subscribe_10.State == false)
            {
            Steamworks.SteamUserStats.SetStat("subscriptions", totalSubscribedItemsCount);
            Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_SUBSCRIBE_10", Steamworks.SteamUserStats.GetStatInt("subscriptions"), 10);
            }
            else if(ach_subscribe_100.State == false)
            {
            Steamworks.SteamUserStats.SetStat("subscriptions", totalSubscribedItemsCount);
            Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_SUBSCRIBE_100", Steamworks.SteamUserStats.GetStatInt("subscriptions"), 25);
            }
            else if(ach_subscribe_1000.State == false)
            {
            Steamworks.SteamUserStats.SetStat("subscriptions", totalSubscribedItemsCount);
            Steamworks.SteamUserStats.IndicateAchievementProgress("ACH_SUBSCRIBE_1000", Steamworks.SteamUserStats.GetStatInt("subscriptions"), 50);
            }

    }

}
