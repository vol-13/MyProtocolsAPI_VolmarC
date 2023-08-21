using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MyProtocolsAPI_VolmarC.Attributes;
using MyProtocolsAPI_VolmarC.Models;
using MyProtocolsAPI_VolmarC.ModelsDTOs;

namespace MyProtocolsAPI_VolmarC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiKey]
    public class UsersController : ControllerBase
    {
        private readonly MyProtocolsDBContext _context;

        public UsersController(MyProtocolsDBContext context)
        {
            _context = context;
        }

        //Este get valida el usuario que quiere ingresar en la app. 
        //GET: api/Users
        [HttpGet("ValidateLogin")]
        public async Task<ActionResult<User>> ValidateLogin(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(e => e.Email.Equals(username) &&
                                                                 e.Password == password);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }



        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpGet("GetUserInfoByEmail")]
        public ActionResult<IEnumerable<UserDTO>> GetUserInfoByEmail(string Pemail)
        {
            //acá creamos un linq que combina información de dos entidades 
            //(user inner join userrole) y la agreaga en el objeto dto de usuario 

            var query = (from u in _context.Users
                         join ur in _context.UserRoles on
                         u.UserRoleId equals ur.UserRoleId
                         where u.Email == Pemail && u.Active == true &&
                         u.IsBlocked == false
                         select new
                         {
                             idusuario = u.UserId,
                             correo = u.Email,
                             contrasennia = u.Password,
                             nombre = u.Name,
                             correorespaldo = u.BackUpEmail,
                             telefono = u.PhoneNumber,
                             direccion = u.Address,
                             activo = u.Active,
                             establoqueado = u.IsBlocked,
                             idrol = ur.UserRoleId,
                             descripcionrol = ur.Description
                         }).ToList();

            //creamos un objeto del tipo que retorna la función 
            List<UserDTO> list = new List<UserDTO>();

            foreach (var item in query)
            {
                UserDTO NewItem = new UserDTO()
                {
                    IDUsuario = item.idusuario,
                    Correo = item.correo,
                    Contrasennia = item.contrasennia,
                    Nombre = item.nombre,
                    CorreoRespaldo = item.correorespaldo,
                    Telefono = item.telefono,
                    Direccion = item.direccion,
                    Activo = item.activo,
                    EstaBloqueado = item.establoqueado,
                    IDRol = item.idrol,
                    DescripcionRol = item.descripcionrol
                };

                list.Add(NewItem);
            }

            if (list == null) { return NotFound(); }

            return list;

        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO user)
        {
            if (id != user.IDUsuario)
            {
                return BadRequest();
            }

            //tenemos que hacer la conversión entre el DTO que llega en formato 
            //json (en el header) y el objeto que entity framework entiende que es de
            //tipo User

            User? NewEFUser = GetUserByID(id);

            if (NewEFUser != null)
            {
                NewEFUser.Email = user.Correo;
                NewEFUser.Name = user.Nombre;
                NewEFUser.BackUpEmail = user.CorreoRespaldo;
                NewEFUser.PhoneNumber = user.Telefono;
                NewEFUser.Address = user.Direccion;
                NewEFUser.Password = user.Contrasennia;

                _context.Entry(NewEFUser).State = EntityState.Modified;

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {

            if (_context.Users == null)
            {
                return Problem("Entity set 'MyProtocolsDBContext.Users'  is null.");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }


        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        private User? GetUserByID(int id)
        {
            var user = _context.Users.Find(id);

            //var user = _context.Users?.Any(e => e.UserId == id);

            return user;
        }

    }
}
