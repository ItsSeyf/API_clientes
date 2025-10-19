using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiAPI.Models;
using NuGet.Protocol.Plugins;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.AspNetCore.Authorization;

namespace MiAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly string _context;

        public ClientesController(IConfiguration configuration)
        {
            _context = configuration.GetConnectionString("Default");
        }

        [HttpGet]
        [Route("Lista")]
        public async Task<IActionResult> Lista([FromServices] Contexto db)
        {
            try
            {
                var clientes = db.Clientes.ToList();
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", Response = clientes });
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message, Response = "[]" });
            }
        }

        [HttpGet]
        [Route("Detalles/{id:int}")]
        public async Task<IActionResult> Detalles(int id, [FromServices] Contexto db)
        {
            try
            {
                var cliente = await db.Clientes.FindAsync(id);
                if (cliente != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", objeto = cliente });
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", objeto = "[]" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message, Response = "[]" });
            }
        }
        [HttpPost]
        [Route("Guardar")]
        public async Task<IActionResult> Guardar([FromServices] Contexto db, [FromBody] Clientes cliente)
        {
            try
            {
                await db.Clientes.AddAsync(cliente);
                await db.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", Response = "[]" });
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message, Response = "[]" });
            }
        }

        [HttpPut]
        [Route("Editar")]
        public async Task<IActionResult> Editar([FromBody] Clientes cliente, [FromServices] Contexto db)
        {
            try
            {
                var clienteEncontrado = await db.Clientes.FindAsync(cliente.id);
                if (clienteEncontrado == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Usuario no encontrado", Response = "[]" });
                }
                db.Entry(clienteEncontrado).CurrentValues.SetValues(cliente);
                await db.SaveChangesAsync();
                return Ok(new { mensaje = "Ok", Response = "[]" });
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message, Response = "[]" });
            }
        }
        [HttpDelete]
        [Route("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id, [FromServices] Contexto db)
        {
            try
            {
                var cliente = await db.Clientes.FindAsync(id);
                if (cliente != null)
                {
                    db.Remove(cliente);
                    await db.SaveChangesAsync();
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK" });
                }
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Usuario no encontrado", Response = "[]" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message, Response = "[]" });
            }
        }
    }
}
