namespace Qydha.API.Mappers;

[Mapper]
public partial class InfluencerCodeMapper
{
    [MapProperty(nameof(InfluencerCodeUserLink.InfluencerCodeId), nameof(UserInfluencerCodeDto.InfluencerCodeId))]
    [MapProperty(nameof(InfluencerCodeUserLink.UsedAt), nameof(UserInfluencerCodeDto.UsedAt))]
    [MapProperty([nameof(InfluencerCodeUserLink.InfluencerCode), nameof(InfluencerCodeUserLink.InfluencerCode.Code)], [nameof(UserInfluencerCodeDto.InfluencerCodeName)])]
    [MapProperty([nameof(InfluencerCodeUserLink.InfluencerCode), nameof(InfluencerCodeUserLink.InfluencerCode.CreatedAt)], [nameof(UserInfluencerCodeDto.CreatedAt)])]
    [MapProperty([nameof(InfluencerCodeUserLink.InfluencerCode), nameof(InfluencerCodeUserLink.InfluencerCode.ExpireAt)], [nameof(UserInfluencerCodeDto.ExpireAt)])]
    [MapProperty([nameof(InfluencerCodeUserLink.InfluencerCode), nameof(InfluencerCodeUserLink.InfluencerCode.NumberOfDays)], [nameof(UserInfluencerCodeDto.NumberOfDays)])]
    [MapProperty([nameof(InfluencerCodeUserLink.InfluencerCode), nameof(InfluencerCodeUserLink.InfluencerCode.Category), nameof(InfluencerCodeUserLink.InfluencerCode.Category.CategoryName)], [nameof(UserInfluencerCodeDto.Category), nameof(UserInfluencerCodeDto.Category.Name)])]
    [MapProperty([nameof(InfluencerCodeUserLink.InfluencerCode), nameof(InfluencerCodeUserLink.InfluencerCode.Category), nameof(InfluencerCodeUserLink.InfluencerCode.Category.Id)], [nameof(UserInfluencerCodeDto.Category), nameof(UserInfluencerCodeDto.Category.Id)])]
    [MapperIgnoreSource(nameof(InfluencerCodeUserLink.UserId))]
    public partial UserInfluencerCodeDto InfluencerCodeUserLinkToInfluencerCodeUsedByUser(InfluencerCodeUserLink link);
}