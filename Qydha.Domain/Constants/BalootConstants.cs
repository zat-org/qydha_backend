using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Qydha.Domain.Constants;
public static class BalootConstants
{
    public static readonly Dictionary<SunMoshtaraScoresId, SunMoshtaraScore> SunRoundScores = new(){
        {SunMoshtaraScoresId.khosaretKaboot , new SunMoshtaraScore(){DisplayValue = "خسارة كبوت" , Name="khosaretKaboot" , Value = 0, GoesToIds= [SunMoshtaraScoresId.Kaboot]}},
        {SunMoshtaraScoresId.khosara , new SunMoshtaraScore(){DisplayValue = "خسارة" , Name="khosara" , Value = 0, GoesToIds= [SunMoshtaraScoresId.Num_26 ,SunMoshtaraScoresId.DoubleSun]}},
        {SunMoshtaraScoresId.Num_0 , new SunMoshtaraScore(){DisplayValue = "0" , Name="Num_0" , Value = 0, GoesToIds= [SunMoshtaraScoresId.Num_26 , SunMoshtaraScoresId.Kaboot]}},
        {SunMoshtaraScoresId.Num_1 , new SunMoshtaraScore(){DisplayValue = "1" , Name="Num_1" , Value = 1, GoesToIds= [SunMoshtaraScoresId.Num_25]}},
        {SunMoshtaraScoresId.Num_2 , new SunMoshtaraScore(){DisplayValue = "2" , Name="Num_2" , Value = 2, GoesToIds= [SunMoshtaraScoresId.Num_24]}},
        {SunMoshtaraScoresId.Num_3 , new SunMoshtaraScore(){DisplayValue = "3" , Name="Num_3" , Value = 3, GoesToIds= [SunMoshtaraScoresId.Num_23]}},
        {SunMoshtaraScoresId.Num_4 , new SunMoshtaraScore(){DisplayValue = "4" , Name="Num_4" , Value = 4, GoesToIds= [SunMoshtaraScoresId.Num_22]}},
        {SunMoshtaraScoresId.Num_5 , new SunMoshtaraScore(){DisplayValue = "5" , Name="Num_5" , Value = 5, GoesToIds= [SunMoshtaraScoresId.Num_21]}},
        {SunMoshtaraScoresId.Num_6 , new SunMoshtaraScore(){DisplayValue = "6" , Name="Num_6" , Value = 6, GoesToIds= [SunMoshtaraScoresId.Num_20]}},
        {SunMoshtaraScoresId.Num_7 , new SunMoshtaraScore(){DisplayValue = "7" , Name="Num_7" , Value = 7, GoesToIds= [SunMoshtaraScoresId.Num_19]}},
        {SunMoshtaraScoresId.Num_8 , new SunMoshtaraScore(){DisplayValue = "8" , Name="Num_8" , Value = 8, GoesToIds= [SunMoshtaraScoresId.Num_18]}},
        {SunMoshtaraScoresId.Num_9 , new SunMoshtaraScore(){DisplayValue = "9" , Name="Num_9" , Value = 9, GoesToIds= [SunMoshtaraScoresId.Num_17]}},
        {SunMoshtaraScoresId.Num_10 , new SunMoshtaraScore(){DisplayValue = "10" , Name="Num_10" , Value = 10, GoesToIds= [SunMoshtaraScoresId.Num_16]}},
        {SunMoshtaraScoresId.Num_11 , new SunMoshtaraScore(){DisplayValue = "11" , Name="Num_11" , Value = 11, GoesToIds= [SunMoshtaraScoresId.Num_15]}},
        {SunMoshtaraScoresId.Num_12 , new SunMoshtaraScore(){DisplayValue = "12" , Name="Num_12" , Value = 12, GoesToIds= [SunMoshtaraScoresId.Num_14]}},
        {SunMoshtaraScoresId.Num_13 , new SunMoshtaraScore(){DisplayValue = "13" , Name="Num_13" , Value = 13, GoesToIds= [SunMoshtaraScoresId.Num_13]}},
        {SunMoshtaraScoresId.Num_14 , new SunMoshtaraScore(){DisplayValue = "14" , Name="Num_14" , Value = 14, GoesToIds= [SunMoshtaraScoresId.Num_12]}},
        {SunMoshtaraScoresId.Num_15 , new SunMoshtaraScore(){DisplayValue = "15" , Name="Num_15" , Value = 15, GoesToIds= [SunMoshtaraScoresId.Num_11]}},
        {SunMoshtaraScoresId.Num_16 , new SunMoshtaraScore(){DisplayValue = "16" , Name="Num_16" , Value = 16, GoesToIds= [SunMoshtaraScoresId.Num_10]}},
        {SunMoshtaraScoresId.Num_17 , new SunMoshtaraScore(){DisplayValue = "17" , Name="Num_17" , Value = 17, GoesToIds= [SunMoshtaraScoresId.Num_9]}},
        {SunMoshtaraScoresId.Num_18 , new SunMoshtaraScore(){DisplayValue = "18" , Name="Num_18" , Value = 18, GoesToIds= [SunMoshtaraScoresId.Num_8]}},
        {SunMoshtaraScoresId.Num_19 , new SunMoshtaraScore(){DisplayValue = "19" , Name="Num_19" , Value = 19, GoesToIds= [SunMoshtaraScoresId.Num_7]}},
        {SunMoshtaraScoresId.Num_20 , new SunMoshtaraScore(){DisplayValue = "20" , Name="Num_20" , Value = 20, GoesToIds= [SunMoshtaraScoresId.Num_6]}},
        {SunMoshtaraScoresId.Num_21 , new SunMoshtaraScore(){DisplayValue = "21" , Name="Num_21" , Value = 21, GoesToIds= [SunMoshtaraScoresId.Num_5]}},
        {SunMoshtaraScoresId.Num_22 , new SunMoshtaraScore(){DisplayValue = "22" , Name="Num_22" , Value = 22, GoesToIds= [SunMoshtaraScoresId.Num_4]}},
        {SunMoshtaraScoresId.Num_23 , new SunMoshtaraScore(){DisplayValue = "23" , Name="Num_23" , Value = 23, GoesToIds= [SunMoshtaraScoresId.Num_3]}},
        {SunMoshtaraScoresId.Num_24 , new SunMoshtaraScore(){DisplayValue = "24" , Name="Num_24" , Value = 24, GoesToIds= [SunMoshtaraScoresId.Num_2]}},
        {SunMoshtaraScoresId.Num_25 , new SunMoshtaraScore(){DisplayValue = "25" , Name="Num_25" , Value = 25, GoesToIds= [SunMoshtaraScoresId.Num_1]}},
        {SunMoshtaraScoresId.Num_26 , new SunMoshtaraScore(){DisplayValue = "26" , Name="Num_26" , Value = 26, GoesToIds= [SunMoshtaraScoresId.Num_0 , SunMoshtaraScoresId.khosara]}},
        {SunMoshtaraScoresId.Kaboot , new SunMoshtaraScore(){DisplayValue = "كبوت" , Name="Kaboot" , Value = 44, GoesToIds= [SunMoshtaraScoresId.Num_0 , SunMoshtaraScoresId.khosaretKaboot]}},
        {SunMoshtaraScoresId.DoubleSun , new SunMoshtaraScore(){DisplayValue = "دبل صن" , Name="DoubleSun" , Value = 52, GoesToIds= [SunMoshtaraScoresId.khosara]}},
    };
    public static readonly Dictionary<HokmMoshtaraScoresId, HokmMoshtaraScore> HokmRoundScores = new(){
        {HokmMoshtaraScoresId.khosaretKaboot , new HokmMoshtaraScore(){DisplayValue = "خسارة كبوت" , Name="khosaretKaboot" , Value = 0 , GoesToIds=[HokmMoshtaraScoresId.Kaboot]}},
        {HokmMoshtaraScoresId.khosara , new HokmMoshtaraScore(){DisplayValue = "خسارة" , Name="khosara" , Value = 0 , GoesToIds=[HokmMoshtaraScoresId.Num_16 ,HokmMoshtaraScoresId.Double,HokmMoshtaraScoresId.Three , HokmMoshtaraScoresId.Four,HokmMoshtaraScoresId.Kahwa]}},
        {HokmMoshtaraScoresId.Num_0 , new HokmMoshtaraScore(){DisplayValue = "0" , Name="Num_0" , Value = 0 , GoesToIds=[HokmMoshtaraScoresId.Num_16 , HokmMoshtaraScoresId.Kaboot]}},
        {HokmMoshtaraScoresId.Num_1 , new HokmMoshtaraScore(){DisplayValue = "1" , Name="Num_1" , Value = 1 , GoesToIds=[HokmMoshtaraScoresId.Num_15]}},
        {HokmMoshtaraScoresId.Num_2 , new HokmMoshtaraScore(){DisplayValue = "2" , Name="Num_2" , Value = 2 , GoesToIds=[HokmMoshtaraScoresId.Num_14]}},
        {HokmMoshtaraScoresId.Num_3 , new HokmMoshtaraScore(){DisplayValue = "3" , Name="Num_3" , Value = 3 , GoesToIds=[HokmMoshtaraScoresId.Num_13]}},
        {HokmMoshtaraScoresId.Num_4 , new HokmMoshtaraScore(){DisplayValue = "4" , Name="Num_4" , Value = 4 , GoesToIds=[HokmMoshtaraScoresId.Num_12]}},
        {HokmMoshtaraScoresId.Num_5 , new HokmMoshtaraScore(){DisplayValue = "5" , Name="Num_5" , Value = 5 , GoesToIds=[HokmMoshtaraScoresId.Num_11]}},
        {HokmMoshtaraScoresId.Num_6 , new HokmMoshtaraScore(){DisplayValue = "6" , Name="Num_6" , Value = 6 , GoesToIds=[HokmMoshtaraScoresId.Num_10]}},
        {HokmMoshtaraScoresId.Num_7 , new HokmMoshtaraScore(){DisplayValue = "7" , Name="Num_7" , Value = 7 , GoesToIds=[HokmMoshtaraScoresId.Num_9]}},
        {HokmMoshtaraScoresId.Num_8 , new HokmMoshtaraScore(){DisplayValue = "8" , Name="Num_8" , Value = 8 , GoesToIds=[HokmMoshtaraScoresId.Num_8]}},
        {HokmMoshtaraScoresId.Num_9 , new HokmMoshtaraScore(){DisplayValue = "9" , Name="Num_9" , Value = 9 , GoesToIds=[HokmMoshtaraScoresId.Num_7]}},
        {HokmMoshtaraScoresId.Num_10 , new HokmMoshtaraScore(){DisplayValue = "10" , Name="Num_10" , Value = 10 , GoesToIds=[HokmMoshtaraScoresId.Num_6]}},
        {HokmMoshtaraScoresId.Num_11 , new HokmMoshtaraScore(){DisplayValue = "11" , Name="Num_11" , Value = 11 , GoesToIds=[HokmMoshtaraScoresId.Num_5]}},
        {HokmMoshtaraScoresId.Num_12 , new HokmMoshtaraScore(){DisplayValue = "12" , Name="Num_12" , Value = 12 , GoesToIds=[HokmMoshtaraScoresId.Num_4]}},
        {HokmMoshtaraScoresId.Num_13 , new HokmMoshtaraScore(){DisplayValue = "13" , Name="Num_13" , Value = 13 , GoesToIds=[HokmMoshtaraScoresId.Num_3]}},
        {HokmMoshtaraScoresId.Num_14 , new HokmMoshtaraScore(){DisplayValue = "14" , Name="Num_14" , Value = 14 , GoesToIds=[HokmMoshtaraScoresId.Num_2]}},
        {HokmMoshtaraScoresId.Num_15 , new HokmMoshtaraScore(){DisplayValue = "15" , Name="Num_15" , Value = 15 , GoesToIds=[HokmMoshtaraScoresId.Num_1]}},
        {HokmMoshtaraScoresId.Num_16 , new HokmMoshtaraScore(){DisplayValue = "16" , Name="Num_16" , Value = 16 , GoesToIds=[HokmMoshtaraScoresId.Num_0 , HokmMoshtaraScoresId.khosara]}},
        {HokmMoshtaraScoresId.Kaboot , new HokmMoshtaraScore(){DisplayValue = "كبوت" , Name="Kaboot" , Value = 25 , GoesToIds=[HokmMoshtaraScoresId.Num_0 ,HokmMoshtaraScoresId.khosaretKaboot ]}},
        {HokmMoshtaraScoresId.Double , new HokmMoshtaraScore(){DisplayValue = "دبل" , Name="Double" , Value = 32 , GoesToIds=[HokmMoshtaraScoresId.khosara]}},
        {HokmMoshtaraScoresId.Three , new HokmMoshtaraScore(){DisplayValue = "ثرى" , Name="Three" , Value = 48 , GoesToIds=[HokmMoshtaraScoresId.khosara]}},
        {HokmMoshtaraScoresId.Four , new HokmMoshtaraScore(){DisplayValue = "فور" , Name="Four" , Value = 64 , GoesToIds=[HokmMoshtaraScoresId.khosara]}},
        {HokmMoshtaraScoresId.Kahwa , new HokmMoshtaraScore(){DisplayValue = "قهوة" , Name="Kahwa" , Value = 152 , GoesToIds=[HokmMoshtaraScoresId.khosara]}},
    };

    public static readonly int[] SakkaCountPerGameValues = [1, 3, 5, 7, 9, 11];

    public static readonly JsonSerializerSettings balootEventsSerializationSettings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
        TypeNameHandling = TypeNameHandling.Auto,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        Converters = [new StringEnumConverter()],
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    public static readonly IDictionary<string, int> Mashare3SunValues = new Dictionary<string, int>() {
        { "Sra", 4 },
        { "Khamsen", 10 },
        { "Me2a", 20 },
        { "Rob3ome2a", 40 },
    };
    public static readonly IDictionary<string, int> Mashare3HokmValues = new Dictionary<string, int>() {
            { "Sra", 2 },
            { "Khamsen", 5 },
            { "Me2a", 10 },
            { "Baloot", 2 },
        };
}
