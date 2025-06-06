namespace BHDTest.DTOs
{
    public class ErrorResponseDto
    {
        public string Mensaje {  get; set; } = string.Empty;
        public ErrorResponseDto(string mensaje) {
            Mensaje = mensaje;        
        }

    }
}
