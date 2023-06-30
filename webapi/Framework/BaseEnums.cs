namespace webapi.Framework.BaseEnums
{
    public enum SysModule
    {
        Public,
        Managment
    }

    public enum PermissionLevel
    {
        Public = 0,
        Client = 20,
        Technic = 40,
        Manager = 60,
        Diretor = 80,
        Admin = 100
    }

    public enum RowState
    {
        Invalid,
        Valid
    }

    /// <summary>
    /// Sistema de abertura
    /// </summary>
    public enum WindowOpeningType
    {
        /// <summary>
        /// Fixo
        /// </summary>
        Fixed = 0,
        /// <summary>
        /// Batente
        /// </summary>
        SideHung = 1,
        /// <summary>
        /// Basculante
        /// </summary>
        TiltOnly = 2,
        /// <summary>
        /// Oscilobatente
        /// </summary>
        TiltAndTurn = 3,
        /// <summary>
        /// Oscilo-paralelo
        /// </summary>
        TiltAndParallel = 4,
    }

    /// <summary>
    /// Direção da abertura
    /// </summary>
    public enum WindowOpeningDirection
    {
        None = 0,
        LeftRight = 1,
        RightLeft = 2,
        Tilt = 3
    }

    /// <summary>
    /// Tipo da cor
    /// </summary>
    public enum ColorType
    {
        Solid = 0,
        Pattern = 1
    }
}
