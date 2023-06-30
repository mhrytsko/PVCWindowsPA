import { toArray } from 'lodash-es'

export const PermissionLevel =
{
    Public: { value: 0, text: "Publico" },
    Client: { value: 20, text: "Cliente" },
    Technic: { value: 40, text: "Técnico" },
    Manager: { value: 60, text: "Gestor" },
    Diretor: { value: 80, text: "Diretor" },
    Admin: { value: 100, text: "Administrador" }
}

export const WindowOpeningType = 
{
    Fixed: { value: 0, text: "Fixo" },
    SideHung: { value: 1, text: "De abrir" },
    TiltOnly: { value: 2, text: "Basculante" },
    TiltAndTurn: { value: 3, text: "Oscilobatente" }
}

export const WindowOpeningDirection =
{
    None: { value: 0, text: "Nenuma" },
    LeftRight: { value: 1, text: "Esquerda para direita" },
    RightLeft: { value: 2, text: "Direita para esquerda" },
    Tilt: { value: 3, text: "Basculante" }
}

export const ColorType =
{
    Solid: { value: 0, text: "Sólida" },
    Pattern: { value: 1, text: "Padrão" },
}


export default {
    PermissionLevel,
    WindowOpeningType,
    WindowOpeningDirection,
    ColorType,

    PermissionLevelAsArray: toArray(PermissionLevel),
    WindowOpeningTypeAsArray: toArray(WindowOpeningType),
    WindowOpeningDirectionAsArray: toArray(WindowOpeningDirection),
    ColorTypeAsArray: toArray(ColorType)
}