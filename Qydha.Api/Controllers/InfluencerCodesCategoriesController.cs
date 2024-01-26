namespace Qydha.API.Controllers;

[ApiController]
[Route("influencer-codes-categories/")]
[Auth(SystemUserRoles.Admin)]
public class InfluencerCodesCategoriesController(IInfluencerCodeCategoryService influencerCodeCategoryService) : ControllerBase
{
    private readonly IInfluencerCodeCategoryService _influencerCodeCategoryService = influencerCodeCategoryService;

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] InfluencerCodeCategoryDto dto)
    {
        var mapper = new InfluencerCodeCategoryMapper();
        return (await _influencerCodeCategoryService.Add(mapper.InfluencerCodeCategoryFromAddDto(dto)))
        .Handle<InfluencerCodeCategory, IActionResult>((infCodeCategory) => Ok(new
        {
            Data = infCodeCategory,
            message = "Influencer Code Category Added Successfully."
        }), BadRequest);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        return (await _influencerCodeCategoryService.GetAll())
        .Handle<IEnumerable<InfluencerCodeCategory>, IActionResult>((infCodeCategories) => Ok(new
        {
            Data = infCodeCategories,
            message = "Influencer Code Categories Fetched Successfully."
        }), BadRequest);
    }

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int categoryId)
    {
        return (await _influencerCodeCategoryService.Delete(categoryId))
        .Handle<IActionResult>(() => Ok(new
        {
            Data = new { },
            message = "Influencer Code Category Deleted Successfully."
        }), BadRequest);
    }
    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] int categoryId, [FromBody] InfluencerCodeCategoryDto dto)
    {
        var mapper = new InfluencerCodeCategoryMapper();
        var category = mapper.InfluencerCodeCategoryFromAddDto(dto);
        category.Id = categoryId;
        return (await _influencerCodeCategoryService.Update(category))
        .Handle<InfluencerCodeCategory, IActionResult>((infCodeCategory) => Ok(new
        {
            Data = infCodeCategory,
            message = "Influencer Code Categories Updated Successfully."
        }), BadRequest);
    }
}
