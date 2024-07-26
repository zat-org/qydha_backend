using NetTopologySuite.IO.Converters;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Qydha.Domain.Constants;
public static class BalootConstants
{
    public static readonly Dictionary<SunMoshtaraScoresId, SunMoshtaraScore> SunRoundScores = new(){
        {SunMoshtaraScoresId.khosaretKaboot , new SunMoshtaraScore( "خسارة كبوت" ,"khosaretKaboot" ,  0,  [SunMoshtaraScoresId.Kaboot])},
        {SunMoshtaraScoresId.khosara , new SunMoshtaraScore( "خسارة" ,"khosara" ,  0,  [SunMoshtaraScoresId.Num_26 ,SunMoshtaraScoresId.DoubleSun])},
        {SunMoshtaraScoresId.Num_0 , new SunMoshtaraScore( "0" ,"Num_0" ,  0,  [SunMoshtaraScoresId.Num_26 , SunMoshtaraScoresId.Kaboot])},
        {SunMoshtaraScoresId.Num_1 , new SunMoshtaraScore( "1" ,"Num_1" ,  1,  [SunMoshtaraScoresId.Num_25])},
        {SunMoshtaraScoresId.Num_2 , new SunMoshtaraScore( "2" ,"Num_2" ,  2,  [SunMoshtaraScoresId.Num_24])},
        {SunMoshtaraScoresId.Num_3 , new SunMoshtaraScore( "3" ,"Num_3" ,  3,  [SunMoshtaraScoresId.Num_23])},
        {SunMoshtaraScoresId.Num_4 , new SunMoshtaraScore( "4" ,"Num_4" ,  4,  [SunMoshtaraScoresId.Num_22])},
        {SunMoshtaraScoresId.Num_5 , new SunMoshtaraScore( "5" ,"Num_5" ,  5,  [SunMoshtaraScoresId.Num_21])},
        {SunMoshtaraScoresId.Num_6 , new SunMoshtaraScore( "6" ,"Num_6" ,  6,  [SunMoshtaraScoresId.Num_20])},
        {SunMoshtaraScoresId.Num_7 , new SunMoshtaraScore( "7" ,"Num_7" ,  7,  [SunMoshtaraScoresId.Num_19])},
        {SunMoshtaraScoresId.Num_8 , new SunMoshtaraScore( "8" ,"Num_8" ,  8,  [SunMoshtaraScoresId.Num_18])},
        {SunMoshtaraScoresId.Num_9 , new SunMoshtaraScore( "9" ,"Num_9" ,  9,  [SunMoshtaraScoresId.Num_17])},
        {SunMoshtaraScoresId.Num_10 , new SunMoshtaraScore( "10" ,"Num_10" ,  10,  [SunMoshtaraScoresId.Num_16])},
        {SunMoshtaraScoresId.Num_11 , new SunMoshtaraScore( "11" ,"Num_11" ,  11,  [SunMoshtaraScoresId.Num_15])},
        {SunMoshtaraScoresId.Num_12 , new SunMoshtaraScore( "12" ,"Num_12" ,  12,  [SunMoshtaraScoresId.Num_14])},
        {SunMoshtaraScoresId.Num_13 , new SunMoshtaraScore( "13" ,"Num_13" ,  13,  [SunMoshtaraScoresId.Num_13])},
        {SunMoshtaraScoresId.Num_14 , new SunMoshtaraScore( "14" ,"Num_14" ,  14,  [SunMoshtaraScoresId.Num_12])},
        {SunMoshtaraScoresId.Num_15 , new SunMoshtaraScore( "15" ,"Num_15" ,  15,  [SunMoshtaraScoresId.Num_11])},
        {SunMoshtaraScoresId.Num_16 , new SunMoshtaraScore( "16" ,"Num_16" ,  16,  [SunMoshtaraScoresId.Num_10])},
        {SunMoshtaraScoresId.Num_17 , new SunMoshtaraScore( "17" ,"Num_17" ,  17,  [SunMoshtaraScoresId.Num_9])},
        {SunMoshtaraScoresId.Num_18 , new SunMoshtaraScore( "18" ,"Num_18" ,  18,  [SunMoshtaraScoresId.Num_8])},
        {SunMoshtaraScoresId.Num_19 , new SunMoshtaraScore( "19" ,"Num_19" ,  19,  [SunMoshtaraScoresId.Num_7])},
        {SunMoshtaraScoresId.Num_20 , new SunMoshtaraScore( "20" ,"Num_20" ,  20,  [SunMoshtaraScoresId.Num_6])},
        {SunMoshtaraScoresId.Num_21 , new SunMoshtaraScore( "21" ,"Num_21" ,  21,  [SunMoshtaraScoresId.Num_5])},
        {SunMoshtaraScoresId.Num_22 , new SunMoshtaraScore( "22" ,"Num_22" ,  22,  [SunMoshtaraScoresId.Num_4])},
        {SunMoshtaraScoresId.Num_23 , new SunMoshtaraScore( "23" ,"Num_23" ,  23,  [SunMoshtaraScoresId.Num_3])},
        {SunMoshtaraScoresId.Num_24 , new SunMoshtaraScore( "24" ,"Num_24" ,  24,  [SunMoshtaraScoresId.Num_2])},
        {SunMoshtaraScoresId.Num_25 , new SunMoshtaraScore( "25" ,"Num_25" ,  25,  [SunMoshtaraScoresId.Num_1])},
        {SunMoshtaraScoresId.Num_26 , new SunMoshtaraScore( "26" ,"Num_26" ,  26,  [SunMoshtaraScoresId.Num_0 , SunMoshtaraScoresId.khosara])},
        {SunMoshtaraScoresId.Kaboot , new SunMoshtaraScore( "كبوت" ,"Kaboot" ,  44,  [SunMoshtaraScoresId.Num_0 , SunMoshtaraScoresId.khosaretKaboot])},
        {SunMoshtaraScoresId.DoubleSun , new SunMoshtaraScore( "دبل صن" ,"DoubleSun" ,  52,  [SunMoshtaraScoresId.khosara])},
    };
    public static readonly Dictionary<HokmMoshtaraScoresId, HokmMoshtaraScore> HokmRoundScores = new(){
        {HokmMoshtaraScoresId.khosaretKaboot , new HokmMoshtaraScore( "خسارة كبوت" ,"khosaretKaboot" , 0 ,[HokmMoshtaraScoresId.Kaboot])},
        {HokmMoshtaraScoresId.khosara , new HokmMoshtaraScore( "خسارة" ,"khosara" , 0 ,[HokmMoshtaraScoresId.Num_16 ,HokmMoshtaraScoresId.Double,HokmMoshtaraScoresId.Three , HokmMoshtaraScoresId.Four,HokmMoshtaraScoresId.Kahwa])},
        {HokmMoshtaraScoresId.Num_0 , new HokmMoshtaraScore( "0" ,"Num_0" , 0 ,[HokmMoshtaraScoresId.Num_16 , HokmMoshtaraScoresId.Kaboot])},
        {HokmMoshtaraScoresId.Num_1 , new HokmMoshtaraScore( "1" ,"Num_1" , 1 ,[HokmMoshtaraScoresId.Num_15])},
        {HokmMoshtaraScoresId.Num_2 , new HokmMoshtaraScore( "2" ,"Num_2" , 2 ,[HokmMoshtaraScoresId.Num_14])},
        {HokmMoshtaraScoresId.Num_3 , new HokmMoshtaraScore( "3" ,"Num_3" , 3 ,[HokmMoshtaraScoresId.Num_13])},
        {HokmMoshtaraScoresId.Num_4 , new HokmMoshtaraScore( "4" ,"Num_4" , 4 ,[HokmMoshtaraScoresId.Num_12])},
        {HokmMoshtaraScoresId.Num_5 , new HokmMoshtaraScore( "5" ,"Num_5" , 5 ,[HokmMoshtaraScoresId.Num_11])},
        {HokmMoshtaraScoresId.Num_6 , new HokmMoshtaraScore( "6" ,"Num_6" , 6 ,[HokmMoshtaraScoresId.Num_10])},
        {HokmMoshtaraScoresId.Num_7 , new HokmMoshtaraScore( "7" ,"Num_7" , 7 ,[HokmMoshtaraScoresId.Num_9])},
        {HokmMoshtaraScoresId.Num_8 , new HokmMoshtaraScore( "8" ,"Num_8" , 8 ,[HokmMoshtaraScoresId.Num_8])},
        {HokmMoshtaraScoresId.Num_9 , new HokmMoshtaraScore( "9" ,"Num_9" , 9 ,[HokmMoshtaraScoresId.Num_7])},
        {HokmMoshtaraScoresId.Num_10 , new HokmMoshtaraScore( "10" ,"Num_10" , 10 ,[HokmMoshtaraScoresId.Num_6])},
        {HokmMoshtaraScoresId.Num_11 , new HokmMoshtaraScore( "11" ,"Num_11" , 11 ,[HokmMoshtaraScoresId.Num_5])},
        {HokmMoshtaraScoresId.Num_12 , new HokmMoshtaraScore( "12" ,"Num_12" , 12 ,[HokmMoshtaraScoresId.Num_4])},
        {HokmMoshtaraScoresId.Num_13 , new HokmMoshtaraScore( "13" ,"Num_13" , 13 ,[HokmMoshtaraScoresId.Num_3])},
        {HokmMoshtaraScoresId.Num_14 , new HokmMoshtaraScore( "14" ,"Num_14" , 14 ,[HokmMoshtaraScoresId.Num_2])},
        {HokmMoshtaraScoresId.Num_15 , new HokmMoshtaraScore( "15" ,"Num_15" , 15 ,[HokmMoshtaraScoresId.Num_1])},
        {HokmMoshtaraScoresId.Num_16 , new HokmMoshtaraScore( "16" ,"Num_16" , 16 ,[HokmMoshtaraScoresId.Num_0 , HokmMoshtaraScoresId.khosara])},
        {HokmMoshtaraScoresId.Kaboot , new HokmMoshtaraScore( "كبوت" ,"Kaboot" , 25 ,[HokmMoshtaraScoresId.Num_0 ,HokmMoshtaraScoresId.khosaretKaboot ])},
        {HokmMoshtaraScoresId.Double , new HokmMoshtaraScore( "دبل" ,"Double" , 32 ,[HokmMoshtaraScoresId.khosara])},
        {HokmMoshtaraScoresId.Three , new HokmMoshtaraScore( "ثرى" ,"Three" , 48 ,[HokmMoshtaraScoresId.khosara])},
        {HokmMoshtaraScoresId.Four , new HokmMoshtaraScore( "فور" ,"Four" , 64 ,[HokmMoshtaraScoresId.khosara])},
        {HokmMoshtaraScoresId.Kahwa , new HokmMoshtaraScore( "قهوة" ,"Kahwa" , 152 ,[HokmMoshtaraScoresId.khosara])},
    };

    public static readonly int[] SakkaCountPerGameValues = [1, 3, 5, 7, 9, 11];

    public static readonly JsonSerializerSettings balootEventsSerializationSettings = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
        TypeNameHandling = TypeNameHandling.Auto,
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        Converters = [new StringEnumConverter(), new GeometryConverter()],
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
