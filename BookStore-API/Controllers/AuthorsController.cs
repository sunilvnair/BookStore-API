using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// End point for Authors controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public AuthorsController(   IAuthorRepository authorRepository ,
                                    ILoggerService logger ,
                                    IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Function to return all authors details
        /// </summary>
        /// <returns>Ilist - List of AuthorDTO</returns>
        // GET: api/<AuthorsController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getAuthors()
        {
            var location = getControllerDetails();
            try
            {
                _logger.LogInfo($"{location} call Started ");
                var authors = await _authorRepository.Findall();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"{location} Successfully data fetched ");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{location} - {e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Function to return all author details by ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>An author's reord</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getAuthor(int Id)
        {
            var location = getControllerDetails();
            try
            {
                _logger.LogInfo($"{location} call Started for getting author by id {Id}");
                var author = await _authorRepository.FindById(Id);
                if (author==null)
                {
                    _logger.LogWarn($"{location} No data fetched  by id {Id}");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{location} Successfully data  by id {Id}");
                return Ok(response);
            }
            catch (Exception e)
            {

                return internalError($"{location} - {e.Message} - {e.InnerException}");

            }
        }
        /// <summary>
        /// To create author details
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns>Created status with new author details</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize (Roles= "SysAdmin")]
        public async Task<IActionResult> createAuthor([FromBody] AuthorCreateDTO authorDTO)
        {
            var location = getControllerDetails();
            try

            {
                _logger.LogInfo($"{location}  call Started");
                if (authorDTO == null)
                {
                    _logger.LogWarn($"{location}  Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}  Incomplete request was submitted");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return internalError($"{location}  Error while saving data");
                }
                _logger.LogInfo($"{location}  created succesfully");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {

                return internalError($"{location}  - {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// To modify author details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns>No content </returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "SysAdmin")]
        public async Task<IActionResult> updateAuthor(int id , [FromBody] AuthorUpdateDTO authorDTO)
        {
            var location = getControllerDetails();
            try

            {
                _logger.LogInfo($"{location} call Started  for Id - {id}");
                if (id < 1 || authorDTO == null ||  id != authorDTO.Id)
                {
                    _logger.LogWarn($" {location} Empty/wrong id request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location} Incomplete request was submitted");
                    return BadRequest(ModelState);
                }
                var isExist = await _authorRepository.isExists(id);
                if (!isExist)
                {
                    _logger.LogWarn($"{location} No valid  details found for id - {id}");
                    return NotFound();
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return internalError("{location} Error while updating data");
                }
                _logger.LogInfo($"{location} completed succesfully for id - {id}");
                return NoContent ();
            }
            catch (Exception e)
            {

                return internalError($"{location} - {e.Message} - {e.InnerException}");
            }
        }


    /// <summary>
    /// To delete author based on the id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "SysAdmin")]
        public async Task<IActionResult> deleteAuthor(int id)
        {
            var location = getControllerDetails();
            try
            {
                _logger.LogInfo($"{location} Call Started  for Id - {id}");
                if (id<1 )
                {
                    _logger.LogWarn($"{location} Incomplete request was submitted");
                    return BadRequest();
                }
                var isExists = await _authorRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location} No valid  detail found for id - {id}");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                if (author ==null)
                {
                    _logger.LogWarn($"{location} No valid  detail found for id - {id}");
                    return NotFound();
                }
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return internalError($"{location} Error while deleting ");
                }
                _logger.LogInfo($"{location} completed succesfully for id - {id}");
                return NoContent();
            }
            catch (Exception e)
            {

                return internalError($"{location} - {e.Message} - {e.InnerException}");
            }
        }

        private ObjectResult internalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, $"Some error occured,Please contact administrator");
        }
        private string getControllerDetails()
        {
            var contollr = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $" {contollr} - {action}";
        }
    }
}
