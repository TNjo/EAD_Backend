using BackendServices.DTOs;
using BackendServices.Models;
using BackendServices.Services;

namespace BackendServices.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
[ApiController]
[Route("api/[controller]")]
public class RankingController : ControllerBase
{
    private readonly RankingService _rankingService;

    public RankingController(RankingService rankingService)
    {
        _rankingService = rankingService;
    }

    [HttpGet("vendor/{vendorId}")]
    public async Task<IActionResult> GetRankingsByVendorId(string vendorId)
    {
        var rankings = await _rankingService.GetRankingsByVendorId(vendorId);
        if (rankings == null || !rankings.Any())
        {
            return NotFound("No ratings found for this Vendor.");
        }
        return Ok(rankings);
    }
    
    // New API to get all ratings using CustomerId
    [HttpGet("customer/{customerId}/ratings")]
    public async Task<IActionResult> GetRatingsByCustomer(string customerId)
    {
        var rankings = await _rankingService.GetRankingsByCustomer(customerId);
        if (rankings == null || !rankings.Any())
        {
            return NotFound("No ratings found for this customer.");
        }
        return Ok(rankings);
    }

    [HttpPost("create-ranking")]
    public async Task<IActionResult> CreateRanking([FromBody] RankingDTO rankingDTO)
    {
        var ranking = new Ranking
        {
            VendorId = rankingDTO.VendorId,
            Rating = rankingDTO.Rating,
            Comment = rankingDTO.Comment,
            CustomerId = rankingDTO.CustomerId
        };

        await _rankingService.CreateRanking(ranking);
        return CreatedAtAction(nameof(CreateRanking), new { id = ranking.RankingId }, ranking);
    }

    [HttpPut("{rankingId}/comment")]
    public async Task<IActionResult> UpdateComment(string rankingId, [FromBody] string newComment)
    {
        await _rankingService.UpdateComment(rankingId, newComment);
        return NoContent();
    }
}