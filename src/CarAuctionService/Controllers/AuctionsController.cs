using AutoMapper;
using CarAuctionService.Data;
using CarAuctionService.DTOs;
using CarAuctionService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAuctionService.Controllers;

[ApiController]
[Route("api/v1/auction")]
//[Authorize("ApiScope")]
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

        if (!auctions.Any())
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
            .FirstOrDefaultAsync(c => c.Id == id);

        if (auction is null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<AuctionDto>(auction));
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(AuctionDto input)
    {
        var auction = mapper.Map<Auction>(input);
        //ToDo: add current user as seller
        auction.Seller = "test";

        context.Auctons.Add(auction);

        var result = await context.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not save changes to DB");
        }

        return CreatedAtAction(nameof(GetAuction),
                               new { auction.Id },
                               mapper.Map<AuctionDto>(auction));

    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto input)
    {
        var auction = await context.Auctons.Include(c => c.Item).FirstOrDefaultAsync(c => c.Id == id);

        if (auction is null) {
            return NotFound();
        }

        //ToDo: chechk seller == username

        auction.Item.Make = input.Make ?? auction.Item.Make;
        auction.Item.Model = input.Model ?? auction.Item.Model;
        auction.Item.Color = input.Color ?? auction.Item.Color;
        
        var result = await context.SaveChangesAsync() > 0;

        if(result)
        {
            return Ok();
        }

        return BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await context.Auctons.FindAsync(id);

        if (auction is null)
        {
            return NotFound();
        }

        //ToDo: chechk seller

        context.Auctons.Remove(auction);

        var result = await context.SaveChangesAsync() > 0;

        if (result)
        {
            return Ok();
        }

        return BadRequest();
    }
}

