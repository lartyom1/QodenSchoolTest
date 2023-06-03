using System;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    // TODO 4: unauthorized users should receive 401 status code
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
          
        public AccountController(IAccountService accountService)
        { 
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        { 
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            else
            {   ////TODO 3: Get user id from cookie   User.Identity.Name
                Account acc = await _accountService.LoadOrCreateAsync(User.Identity.Name);//TODO6 SOLUTION 
                acc = await _accountService.LoadOrCreateAsync(acc.InternalId);//
                return Ok(acc); 

                //return Ok(await _accountService.LoadOrCreateAsync(User.Identity.Name)); //TODO6 ISSUE
            }        
        }

        [HttpGet("{id}")]
        public IActionResult GetByInternalId([FromRoute] int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            else
            {
                //var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value; //dumb way            
                if (User.IsInRole("Admin"))//TODO 5: Endpoint should works only for users with "Admin" Role
                {
                    //at this point alice counter in cache is 5 //ps i haven't noticed there was difference between by Id and by GuId           
                    return Ok(_accountService.GetFromCache(id));
                }
                else 
                {
                    return StatusCode(403);// aka Forbidden
                }
            }
        }

        [HttpPost("counter")]
        public async Task<IActionResult> UpdateAccount()
        {
            //Update account in cache, don't bother saving to DB, this is not an objective of this task.
            /*
            * TODO 6: Fix issue
            * Users complains that sometimes, when they call AccountController.UpdateAccount followed by
            * AccountController.GetByInternalId they get account with counter equals 0, like if UpdateCounter was never
            * called.
            * It looks like as if there were two accounts, one being updated by UpdateAccount method and another does not.
            * Find out the problem and fix it.
            */

            //Update account is using Get, followed by LoadOrCreateAsync to accquire data
            //Get and GetByInternalId are using diferent methods to accquire data
            //the first one is using loadOrCreateAsync, while second is using GetFromCache
            //and that is not a problem, but i've started my investigation from here

            //both methods take long or string parameters
            //account id - Id long like 1
            //external id - GuID string like alice

            //it is possible to get account by GuId and Id

            //results are NOT equal

            //Update account WAS extracting and incrementing account by GuID
            //GetByInternalId is extracting account by id
            //this leads to unexpected behavior - it looks like the counter is not incrementing, while it is actually incrementing
            //to solve the problem account must be extacted the same way everywhere or the counter must be incremented for both by Id and by Guid

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            else
            {
                var okResult = await Get() as OkObjectResult;
                Account account = okResult.Value as Account;
                account.Counter++;

                return Ok(account);
            }
        }
    }

}