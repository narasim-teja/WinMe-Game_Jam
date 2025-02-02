using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Supabase;
using Postgrest;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using Newtonsoft.Json;
public static class SupaBaseClient
{
    public const string SUPARBASE_URL = "https://ghrfyesekxzbkkyjvkiv.supabase.co";

    // this is a public key, no problem in keeping it out in the open.
    public const string SUPARBASE_PUBLIC_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImdocmZ5ZXNla3h6YmtreWp2a2l2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzcyODEyNDAsImV4cCI6MjA1Mjg1NzI0MH0.hJABFN6o-O9MjFOE3GboR6cLq_-i954wU2GdTPtcXY4";

    private static Supabase.Client supabaseClient = null;

    // Start is called before the first frame update
    private static async void InitializeAsync()
    {
        if(supabaseClient == null){
            supabaseClient = new Supabase.Client(SUPARBASE_URL,SUPARBASE_PUBLIC_KEY);
            await supabaseClient.InitializeAsync();
        }
    }

    public class User : Postgrest.Models.BaseModel
    {
        public string wallet_address { get; set; }
        [Postgrest.Attributes.Column]

        public int coin_count { get; set; }
        public string user_name { get; set; }
    }

    public class Response
    {
        public object ResponseMessage { get; set; }
        public string Content { get; set; }
        public object ClientOptions { get; set; }
        [JsonProperty("$type")]
        public string Type { get; set; }
        

    }
    private static async Task<User> WalletAddressExists(string walletAddress)
    {
        var response = await supabaseClient
            .From<User>()
            .Select("wallet_address , coin_count , user_name").Where(x => x.wallet_address ==walletAddress).Get();

        var responseObj = JsonConvert.DeserializeObject<Response>(response.Serialize().json);

        if(responseObj.Content.Length == 2 ) return null;

        List<User> users = JsonConvert.DeserializeObject<List<User>>(responseObj.Content);
       
        return users[0];
    }
    public static async void addMoneyToDb(string wallet_address, int coin_count , string user_name)
    {
        if(supabaseClient == null) InitializeAsync();
        
        User exsitingUser = await WalletAddressExists(wallet_address);
        
        var data = new User
            {
                wallet_address = wallet_address,
                coin_count = coin_count,
                user_name = user_name
            };
        if(exsitingUser != null ){
            var response = await supabaseClient
                .From<User>()
                .Where(x => x.wallet_address == data.wallet_address)
                .Set(x => x.coin_count, exsitingUser.coin_count + data.coin_count)
                .Update();
        }
        else{
            var response = await supabaseClient.From<User>().Insert(new[] { data });
        }
        

    }

    public static async Task<int> GetCoinCount(string walletAddress)
    {
        if (supabaseClient == null) InitializeAsync();

        User existingUser = await WalletAddressExists(walletAddress);

        if (existingUser != null)
        {
            return existingUser.coin_count; 
        }
        else
        {
            return 0; 
        }
    }
}
