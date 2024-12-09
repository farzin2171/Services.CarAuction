using AutoMapper;
using CarAuctionService.Data;
using CarAuctionService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionService.Controllers;

[ApiController]
[Route("api/v1/auction")]
[Authorize("ApiScope")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext context;
    private readonly IMapper mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await context.Auctons
            .Include(c => c.Item)
            .OrderBy(c => c.Item.Make)
            .ToListAsync();

        if(!auctions.Any())
        {
            return NotFound();
        }
        return Ok(mapper.Map<List<AuctionDto>>(auctions));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
    {
        var auction = await context.Auctons
            .Include(c => c.Item)
            .FirstOrDefaultAsync(c=>c.Id == id);

        if (auction is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<AuctionDto>(auction));
    }
}

