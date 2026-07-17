namespace CampoVerde.Seguridad
{
    public static class Permisos
    {
        // Roles
        public const string SuperAdministrador = "SUPER_ADMINISTRADOR";
        public const string Administrador = "ADMINISTRADOR";
        public const string Veterinario = "VETERINARIO";
        public const string Operario = "OPERARIO";

        // Combinaciones de permisos
        public static readonly string[] Todos =
        {
            SuperAdministrador,
            Administrador,
            Veterinario,
            Operario
        };

        public static readonly string[] Administracion =
        {
            SuperAdministrador,
            Administrador
        };

        public static readonly string[] Sanidad =
        {
            SuperAdministrador,
            Administrador,
            Veterinario
        };

        public static readonly string[] Produccion =
        {
            SuperAdministrador,
            Administrador,
            Operario
        };

        public static readonly string[] Operaciones =
        {
            SuperAdministrador,
            Administrador,
            Veterinario,
            Operario
        };
    }
}