using BankServer_WebAPI.Models;
using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BankServer_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        [HttpGet]
        [Route("users")]
        public IActionResult getUsers()
        {
            try
            {
                List<UserProfile> users = DBManager.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("users/{identifier}")]
        public IActionResult getUserByNameOrEmail(string identifier)
        {
            try
            {
                UserProfile userProfile = DBManager.GetUserProfile(identifier);
                if (userProfile == null)
                {
                    return NotFound();
                }
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("adduser")]
        public IActionResult newUser([FromBody] UserProfile user)
        {
            try
            {
                if (DBManager.InsertUserProfile(user))
                {
                    var response = new { Message = "User added successfully" };
                    return new ObjectResult(response)
                    {
                        StatusCode = 200,
                        ContentTypes = { "application/json" }
                    };
                }
                return BadRequest("Error in user addition");
            }
            catch (SerializationException ex)
            {
                return BadRequest($"Serialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("users/{identifier}")]
        public IActionResult deleteUser(int identifier)
        {
            try
            {
                if (DBManager.DeleteUserProfile(identifier))
                {
                    return Ok("Successfully deleted user.");
                }
                return BadRequest("User could not be deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult updateUser(UserProfile user)
        {
            try
            {
                if (DBManager.UpdateUserProfile(user))
                {
                    return Ok("Successfully updated user profile");
                }
                return BadRequest("Could not update user profile");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("accounts")]
        public IActionResult GetAccounts()
        {
            try
            {
                List<Account> accounts = DBManager.GetAllAccounts();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("accounts/{accountId}")]
        public IActionResult GetAccountById(int accountId)
        {
            try
            {
                Account account = DBManager.GetAccountById(accountId);
                if (account == null)
                {
                    return NotFound();
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("accounts")]
        public IActionResult CreateAccount([FromBody] Account account)
        {
            try
            {
                if (DBManager.InsertAccount(account))
                {
                    return Ok("Successfully created account.");
                }
                return BadRequest("Error in account creation");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("accounts")]
        public IActionResult UpdateAccount(Account account)
        {
            try
            {
                if (DBManager.UpdateAccount(account))
                {
                    return Ok("Successfully updated account.");
                }
                return BadRequest("Could not update account");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("accounts/{accountId}")]
        public IActionResult DeleteAccount(int accountId)
        {
            try
            {
                if (DBManager.DeleteAccount(accountId))
                {
                    return Ok("Successfully deleted account.");
                }
                return BadRequest("Account could not be deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("transactions")]
        public IActionResult GetTransactions()
        {
            try
            {
                List<Transaction> transactions = DBManager.GetAllTransactions();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("transactions/{transactionId}")]
        public IActionResult GetTransactionById(int transactionId)
        {
            try
            {
                Transaction transaction = DBManager.GetTransactionById(transactionId);
                if (transaction == null)
                {
                    return NotFound();
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("transactions")]
        public IActionResult CreateTransaction([FromBody] Transaction transaction)
        {
            try
            {
                if (DBManager.InsertTransaction(transaction))
                {
                    return Ok("Successfully created transaction.");
                }
                return BadRequest("Error in transaction creation");
            }
            catch (SerializationException ex)
            {
                return BadRequest($"Serialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("transactions")]
        public IActionResult UpdateTransaction(Transaction transaction)
        {
            try
            {
                if (DBManager.UpdateTransaction(transaction))
                {
                    return Ok("Successfully updated transaction.");
                }
                return BadRequest("Could not update transaction");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("transactions/{transactionId}")]
        public IActionResult DeleteTransaction(int transactionId)
        {
            try
            {
                if (DBManager.DeleteTransaction(transactionId))
                {
                    return Ok("Successfully deleted transaction.");
                }
                return BadRequest("Transaction could not be deleted");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("transactions/account/{accountId}")]
        public IActionResult GetTransactionsByAccountId(int accountId)
        {
            try
            {
                List<Transaction> transactions = DBManager.GetAllTransactions();
                List<Transaction> returnTransactions = new List<Transaction>();

                foreach (Transaction transaction in transactions)
                {
                    if (transaction.AccountId == accountId)
                    {
                        returnTransactions.Add(transaction);
                    }
                }

                return Ok(returnTransactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

