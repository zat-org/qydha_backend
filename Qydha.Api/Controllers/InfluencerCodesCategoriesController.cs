namespace Qydha.API.Controllers;

[ApiController]
[Route("influencer-codes-categories/")]
[Authorize(Roles = RoleConstants.Admin)]
public class InfluencerCodesCategoriesController(IInfluencerCodeCategoryService influencerCodeCategoryService) : ControllerBase
{
    private readonly IInfluencerCodeCategoryService _influencerCodeCategoryService = influencerCodeCategoryService;

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] InfluencerCodeCategoryDto dto)
    {
        var mapper = new InfluencerCodeCategoryMapper();
        return (await _influencerCodeCategoryService.Add(mapper.InfluencerCodeCategoryFromAddDto(dto)))
        .Resolve((infCodeCategory) => Ok(new
        {
            Data = infCodeCategory,
            message = "Influencer Code Category Added Successfully."
        }), HttpContext.TraceIdentifier);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        return (await _influencerCodeCategoryService.GetAll())
        .Resolve((infCodeCategories) => Ok(new
        {
            Data = infCodeCategories,
            message = "Influencer Code Categories Fetched Successfully."
        }), HttpContext.TraceIdentifier);
    }

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int categoryId)
    {
        return (await _influencerCodeCategoryService.Delete(categoryId))
        .Resolve(() => Ok(new
        {
            Data = new { },
            message = "Influencer Code Category Deleted Successfully."
        }), HttpContext.TraceIdentifier);
    }
    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] int categoryId, [FromBody] InfluencerCodeCategoryDto dto)
    {
        var mapper = new InfluencerCodeCategoryMapper();
        var category = mapper.InfluencerCodeCategoryFromAddDto(dto);
        category.Id = categoryId;
        return (await _influencerCodeCategoryService.Update(category))
        .Resolve((infCodeCategory) => Ok(new
        {
            Data = infCodeCategory,
            message = "Influencer Code Categories Updated Successfully."
        }), HttpContext.TraceIdentifier);
    }
}
