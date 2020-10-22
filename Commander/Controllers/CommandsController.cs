using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Commander.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase

    {
        //private readonly MockCommandRepo _repository=new MockCommandRepo();

        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //GET api/commands
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commandItems = _repository.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }
        //GET api/commands/5
        [HttpGet("{id}", Name = "GetCommandById")]

        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);
            if (commandItem != null)
                return Ok(_mapper.Map<CommandReadDto>(commandItem));
            return NotFound();
        }

        //POST api/commands
        //CommandReadDto as a return type
        //CommandCreateDt as an input type
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            //Convert API request body(Mapping model) into the Coomand domain model
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            //The converted model (Domain model) to the CreateCommand()
            _repository.CreateCommand(commandModel);
            _repository.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);

            //return uri resource
            //localhost:44388/api/Commands/5
            //It is really important to pass back the uir resource in POST method, which is the principle of REST
            //routeName(string), routeValues(Obj), content(Obj)
            return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDto.Id }, commandReadDto);
        }

        //PUT api/commands/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(commandUpdateDto, commandModelFromRepo);
            _repository.UpdateCommandById(commandModelFromRepo);
            _repository.SaveChanges();
            //Return 204
            return NoContent();
        }

        //PATCH api/commands/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> jsonPatchDocument)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }
            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            jsonPatchDocument.ApplyTo(commandToPatch, ModelState);
            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(commandToPatch, commandModelFromRepo);
            _repository.UpdateCommandById(commandModelFromRepo);
            _repository.SaveChanges();
            //Return 204
            return NoContent();
        }

        //DELETE api/commands/{id}
        [HttpDelete("{id}")]

        public ActionResult DeleteCommand(int id)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }
            _repository.DeleteCommand(commandModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }
    }
}
