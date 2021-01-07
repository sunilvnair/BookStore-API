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
    /// End point for Books controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public BooksController(IBookRepository bookRepository,
                                    ILoggerService logger ,
                                    IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Function to return all books details
        /// </summary>
        /// <returns>Ilist - List of BooksDTO</returns>
        // GET: api/<BooksController>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> getBooks()
        {
            var location = getControllerDetails();
            try
            {
                _logger.LogInfo($"{location} Call Started for getting books");
                var books = await _bookRepository.Findall();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{location} Successfully data fetched ");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{location} - {e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Function to return all books details by ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>An book's reord</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> getBook(int Id)
        {
            var location = getControllerDetails();
            try
            {
                _logger.LogInfo($"{location}  Call Started for getting book by id {Id}");
                var book = await _bookRepository.FindById(Id);
                if (book==null)
                {
                    _logger.LogWarn($"{location}  No data fetched for id {Id}");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{location}  Successfully data fetched for  id {Id}");
                return Ok(response);
            }
            catch (Exception e)
            {

                return internalError($"{location}  - {e.Message} - {e.InnerException}");

            }
        }
        /// <summary>
        /// To create book details
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns>Created status with new book details</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> createBook([FromBody] BookCreateDTO bookDTO)
        {
            var location = getControllerDetails();
            try

            {
                _logger.LogInfo($"{location} Call Started ");
                if (bookDTO == null)
                {
                    _logger.LogWarn($"{location} Empty request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location} Incomplete request was submitted");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Create(book);
                if (!isSuccess)
                {
                    return internalError($"{location} Error while saving data");
                }
                _logger.LogInfo($"{location} Created succesfully");
                return Created("Create", new { book });
            }
            catch (Exception e)
            {

                return internalError($"{location} - {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// To modify book details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookDTO"></param>
        /// <returns>No content </returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> updateBook(int id , [FromBody] BookUpdateDTO bookDTO)
        {
            var location = getControllerDetails();
            try

            {
                _logger.LogInfo($"{location} Call Started  for Id - {id}");
                if (id < 1 || bookDTO == null ||  id != bookDTO.Id)
                {
                    _logger.LogWarn($"{location} Empty/wrong id request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}  Incomplete request was submitted");
                    return BadRequest(ModelState);
                }
                var isExist = await _bookRepository.isExists(id);
                if (!isExist)
                {
                    _logger.LogWarn($"{location}  No valid book details found for id - {id}");
                    return NotFound();
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Update(book);
                if (!isSuccess)
                {
                    return internalError($"{location}  Error while updating data");
                }
                _logger.LogInfo($"{location}  completed succesfully for id - {id}");
                return NoContent ();
            }
            catch (Exception e)
            {

                return internalError($"{location}  - {e.Message} - {e.InnerException}");
            }
        }


    /// <summary>
    /// To delete book based on the id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> deleteBook(int id)
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
                var isExists = await _bookRepository.isExists(id);
                if (!isExists)
                {
                    _logger.LogWarn($"{location} No valid book detail found for id - {id}");
                    return NotFound();
                }
                var book = await _bookRepository.FindById(id);
                if (book ==null)
                {
                    _logger.LogWarn($"{location} No valid book detail found for id - {id}");
                    return NotFound();
                }
                var isSuccess = await _bookRepository.Delete(book);
                if (!isSuccess)
                {
                    return internalError($"{location} Error while deleting book");
                }
                _logger.LogInfo($"{location}  completed succesfully for id - {id}");
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

        private string getControllerDetails ()
        {
            var contollr = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $" {contollr} - {action}";
        }
    }
}
