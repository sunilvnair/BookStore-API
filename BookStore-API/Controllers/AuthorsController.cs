using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
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
            try
            {
                _logger.LogInfo("Started getAuthors for getting authors");
                var authors = await _authorRepository.Findall();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Successfully data fetched for getAuthors ");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{e.Message} - {e.InnerException}");
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
            try
            {
                _logger.LogInfo($"Started getAuthor for getting author by id {Id}");
                var author = await _authorRepository.FindById(Id);
                if (author==null)
                {
                    _logger.LogWarn($"No data fetched for getAuthor  by id {Id}");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"Successfully data fetched for getAuthor  by id {Id}");
                return Ok(response);
            }
            catch (Exception e)
            {

                return internalError($"{e.Message} - {e.InnerException}");

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
        public async Task<IActionResult> createAuthor([FromBody] AuthorCreateDTO authorDTO)
        {
            try

            {
                _logger.LogInfo("Started author creation");
                if (authorDTO == null)
                {
                    _logger.LogWarn($"Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Incomplete request was submitted");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return internalError("Error while saving data");
                }
                _logger.LogInfo($"Author created succesfully");
                return Created("Create", new { author });
            }
            catch (Exception e)
            {

                return internalError($"{e.Message} - {e.InnerException}");
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
        public async Task<IActionResult> updateAuthor(int id , [FromBody] AuthorUpdateDTO authorDTO)
        {
            try

            {
                _logger.LogInfo($"Started author updation for Id - {id}");
                if (id < 1 || authorDTO == null ||  id != authorDTO.Id)
                {
                    _logger.LogWarn($"Empty/wrong id request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Incomplete request was submitted");
                    return BadRequest(ModelState);
                }
                var isExist = await _authorRepository.isExists(id);
                if (!isExist)
                {
                    _logger.LogWarn($"No valid author details found for id - {id}");
                    return NotFound();
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return internalError("Error while updating data");
                }
                _logger.LogInfo($"Author updation completed succesfully for id - {id}");
                return NoContent ();
            }
            catch (Exception e)
            {

                return internalError($"{e.Message} - {e.InnerException}");
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
        public async Task<IActionResult> deleteAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Started author deletion for Id - {id}");
                if (id<1 )
                {
                    _logger.LogWarn($"Incomplete request was submitted");
                    return BadRequest();
                }
                var isExists = await _authorRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"No valid author detail found for id - {id}");
                    return NotFound();
                }
                var author = await _authorRepository.FindById(id);
                if (author ==null)
                {
                    _logger.LogWarn($"No valid author detail found for id - {id}");
                    return NotFound();
                }
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return internalError($"Error while deleting author");
                }
                _logger.LogInfo($"Author deletion completed succesfully for id - {id}");
                return NoContent();
            }
            catch (Exception e)
            {

                return internalError($"{e.Message} - {e.InnerException}");
            }
        }

            private ObjectResult internalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, $"Some error occured,Please contact administrator");
        }
    }
}
