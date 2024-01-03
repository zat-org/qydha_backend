namespace Qydha.API.Mappers;
[Mapper]

public partial class AssetsMapper
{
    public partial PopupDto PopUpAssetToPopupDto(PopUpAsset popUpAsset);

    [MapProperty(nameof(PopUpAsset.Image), nameof(GetPopupDto.ImageUrl))]
    public partial GetPopupDto PopUpAssetToGetPopupDto(PopUpAsset popUpAsset);
    private string? FileDataToUrl(FileData? fileData)
    {
        return fileData?.Url;
    }
}
