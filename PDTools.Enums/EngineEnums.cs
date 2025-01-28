using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace PDTools.Enums;

public enum Language : byte
{
    [Description("Japanese")]
    JP = 0,

    [Description("American")]
    US = 1,

    [Description("British (Primary)")]
    GB = 2,

    [Description("French")]
    FR = 3,

    [Description("Spanish")]
    ES = 4,

    [Description("German")]
    DE = 5,

    [Description("Italian")]
    IT = 6,

    [Description("Dutch")]
    NL = 7,

    [Description("Portuguese")]
    PT = 8,

    [Description("Russian")]
    RU = 9,

    [Description("Polish")]
    PL = 10,

    [Description("Turkish")]
    TR = 11,

    [Description("Greek")]
    EL = 12,

    [Description("Korean")]
    KR = 13,

    [Description("Chinese (Taiwan)")]
    TW = 14,

    [Description("Chinese")]
    CN = 15,

    [Description("Japanese")]
    DK = 16,

    [Description("Norwegian")]
    NO = 17,

    [Description("Swedish")]
    SE = 18,

    [Description("Finish")]
    FI = 19,

    [Description("Czech")]
    CZ = 20,

    [Description("Magyar (Hungary)")]
    HU = 21,

    [Description("Portuguese (Brazillian)")]
    BP = 22,

    [Description("Spanish (Mexican)")]
    MS = 23,

    MAX = 24,
    SYSTEM = 25
}

public enum Tuner
{
    nothing = 0,
    acura = 2,
    alfaromeo = 3,
    astonmartin = 4,
    audi = 5,
    bmw = 6,
    chevrolet = 7,
    chrysler = 8,
    citroen = 9,
    daihatsu = 10,
    dodge = 11,
    fiat = 12,
    ford = 13,
    gillet = 14,
    honda = 15,
    hyundai = 16,
    jaguar = 17,
    lancia = 18,
    lister = 19,
    lotus = 20,
    mazda = 21,
    mercedes = 22,
    mg_mini = 23,
    mines = 24,
    mitsubishi = 25,
    mugen = 26,
    nismo = 27,
    nissan = 28,
    opel = 29,
    pagani = 30,
    panoz = 31,
    peugeot = 32,
    polyphony = 33,
    renault = 34,
    ruf = 35,
    shelby = 36,
    spoon = 37,
    subaru = 38,
    suzuki = 39,
    tickford = 40,
    tommykaira = 41,
    toms = 42,
    toyota = 43,
    tvr = 44,
    vauxhall = 45,
    volkswagen = 46,
    asl = 47,
    dome = 48,
    infiniti = 49,
    lexus = 50,
    mini = 51,
    pontiac = 52,
    spyker = 53,
    cadillac = 54,
    plymouth = 55,
    isuzu = 56,
    autobianchi = 57,
    ginetta = 58,
    amuse = 59,
    saleen = 60,
    vemac = 61,
    jayleno = 62,
    buick = 63,
    callaway = 64,
    dmc = 65,
    eagle = 66,
    mercury = 67,
    triumph = 68,
    volvo = 69,
    hommell = 70,
    jensen = 71,
    marcos = 72,
    scion = 73,
    blitz = 74,
    cizeta = 75,
    hks = 76,
    pescarolo = 77,
    fpv = 78,
    opera = 79,
    caterham = 80,
    ac = 81,
    bentley = 82,
    seat = 83,
    landrover = 84,
    holden = 85,
    alpine = 86,
    amemiya = 87,
    nike = 88,
    toyotamodellista = 89,
    trial = 90,
    au_ford = 91,
    trd = 92,
    chaparral = 93,
    hpa = 94,
    autounion = 95,
    proto = 96,
    yamaha = 97,
    kawasaki = 98,
    aprilia = 99,
    mvagusta = 100,
    yoshimura = 101,
    moriwaki = 102,
    buell = 103,
    ducati = 104,
    _7honda = 105,
    ysp_presto = 106,
    trickstar = 107,
    yoshimura_suzuki = 108,
    moriwaki_motul = 109,
    ferrari = 110,
    artmorrison = 111,
    lamborghini = 112,
    bugatti = 113,
    renaultsport = 114,
    hep = 115,
    maserati = 116,
    mclaren = 117,
    aem = 118,
    tesla_motors = 119,
    gtg = 120,
    ktm = 121,
    rocket_r_d = 122,
    stielow_eng = 123,
    pozzi_motorsports = 124,
    abarth = 125,
    fisker = 126,
    srt = 127,
    monstersport = 128,
    ram = 129,
    tajima = 130,
    deltawing = 131,
    hudson = 132,
    ayrton_senna = 133,
    zagato = 134,
}

public enum Country
{
    PDI = 0,
    JP = 2,
    US = 3,
    GB = 4,
    DE = 5,
    FR = 6,
    IT = 7,
    AU = 8,
    KR = 9,
    BE = 10,
    NL = 11,
    SE = 12,
    ES = 13,
    CA = 14,
    AT = 15,
    NumOfCountries = 15
}

public enum DriverType : sbyte
{
    NONE = -1,
    PLAYER = 0,
    AI = 1,
    GPS = 3,
}
