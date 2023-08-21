using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyProtocolsAPI_VolmarC.Models;

namespace MyProtocolsAPI_VolmarC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            //vamos a leer la etiqueta CNNSTR de appsettings.json para configurar la conexion a la basa de datos
            var CnnStrBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("CNNSTR"));

            //Eliminamos del la CNNSTR el dato del password, porque seria sencillo obtener la info de conexion
            //del usuario de SQL server del archivo de config appsettings.json
            CnnStrBuilder.Password = "123456";

            //el cnnstrbuilder es un objeto que permite la construccion de cadenas de conexion a base de datos
            //se puede modificar cada parte de la misma, pero al final debemos extraer un string con la info final
            string cnnStr = CnnStrBuilder.ToString();

            //ahora conectamos el proyecto a la base de datos usando cnnstr
            builder.Services.AddDbContext<MyProtocolsDBContext>(options => options.UseSqlServer(cnnStr));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}