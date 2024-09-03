namespace TransportManager.Models;

public class CityDistances {
  private readonly Dictionary<(string, string), double> _distances = [];

  public CityDistances() {
    // Distâncias existentes
    AddDistance("São Paulo", "Rio de Janeiro", 429);
    AddDistance("São Paulo", "Belo Horizonte", 586);
    AddDistance("São Paulo", "Salvador", 1962);
    AddDistance("São Paulo", "Brasília", 1015);
    AddDistance("São Paulo", "Curitiba", 408);
    AddDistance("São Paulo", "Fortaleza", 2384);
    AddDistance("São Paulo", "Recife", 2660);
    AddDistance("São Paulo", "Porto Alegre", 1109);
    AddDistance("São Paulo", "Manaus", 3890);
    AddDistance("São Paulo", "Goiânia", 926);
    AddDistance("São Paulo", "Belém", 2933);
    AddDistance("São Paulo", "Campinas", 99);
    AddDistance("São Paulo", "São Luís", 2970);
    AddDistance("São Paulo", "Maceió", 2453);
    AddDistance("São Paulo", "Natal", 2618);
    AddDistance("São Paulo", "Teresina", 2579);
    AddDistance("São Paulo", "João Pessoa", 2581);
    AddDistance("São Paulo", "Buenos Aires", 2171);
    AddDistance("São Paulo", "Santiago", 3535);
    AddDistance("São Paulo", "Lima", 3789);
    AddDistance("São Paulo", "Bogotá", 4303);
    AddDistance("São Paulo", "Quito", 4670);
    AddDistance("São Paulo", "Montevideo", 1561);
    AddDistance("São Paulo", "Asunción", 1634);
    AddDistance("São Paulo", "La Paz", 2970);
    AddDistance("São Paulo", "Caracas", 5123);
    AddDistance("São Paulo", "Guadalajara", 7965);
    AddDistance("São Paulo", "Medellín", 4507);
    AddDistance("São Paulo", "Córdoba", 2025);
    AddDistance("São Paulo", "Mar del Plata", 2661);
    AddDistance("São Paulo", "Rosario", 2180);
    AddDistance("São Paulo", "Valparaíso", 3535);
    AddDistance("São Paulo", "Goiânia", 926);
    AddDistance("São Paulo", "Belém", 2933);
    AddDistance("São Paulo", "Campinas", 99);
    AddDistance("São Paulo", "São Luís", 2970);
    AddDistance("São Paulo", "Maceió", 2453);
    AddDistance("São Paulo", "Natal", 2618);
    AddDistance("São Paulo", "Teresina", 2579);
    AddDistance("São Paulo", "João Pessoa", 2581);
    AddDistance("São Paulo", "Aracaju", 2187);
    AddDistance("São Paulo", "Campo Grande", 1014);
    AddDistance("São Paulo", "Cuiabá", 1614);
    AddDistance("São Paulo", "Florianópolis", 705);
    AddDistance("São Paulo", "Vitória", 882);

    // Novas distâncias
    AddDistance("Rio de Janeiro", "Belo Horizonte", 434);
    AddDistance("Rio de Janeiro", "Salvador", 1649);
    AddDistance("Rio de Janeiro", "Brasília", 1148);
    AddDistance("Rio de Janeiro", "Curitiba", 852);
    AddDistance("Rio de Janeiro", "Fortaleza", 2805);
    AddDistance("Rio de Janeiro", "Recife", 2338);
    AddDistance("Rio de Janeiro", "Porto Alegre", 1553);
    AddDistance("Rio de Janeiro", "Manaus", 3992);
    AddDistance("Rio de Janeiro", "Goiânia", 1338);
    AddDistance("Rio de Janeiro", "Belém", 3250);
    AddDistance("Rio de Janeiro", "Campinas", 506);
    AddDistance("Rio de Janeiro", "São Luís", 3015);
    AddDistance("Rio de Janeiro", "Maceió", 2131);
    AddDistance("Rio de Janeiro", "Natal", 2422);
    AddDistance("Rio de Janeiro", "Teresina", 2579);
    AddDistance("Rio de Janeiro", "João Pessoa", 2338);
    AddDistance("Rio de Janeiro", "Belo Horizonte", 434);
    AddDistance("Rio de Janeiro", "Salvador", 1649);
    AddDistance("Rio de Janeiro", "Brasília", 1148);
    AddDistance("Rio de Janeiro", "Curitiba", 852);
    AddDistance("Rio de Janeiro", "Fortaleza", 2805);
    AddDistance("Rio de Janeiro", "Recife", 2338);
    AddDistance("Rio de Janeiro", "Porto Alegre", 1553);
    AddDistance("Rio de Janeiro", "Manaus", 3992);
    AddDistance("Rio de Janeiro", "Buenos Aires", 2332);
    AddDistance("Rio de Janeiro", "Santiago", 3626);
    AddDistance("Rio de Janeiro", "Lima", 3850);
    AddDistance("Rio de Janeiro", "Bogotá", 4432);
    AddDistance("Rio de Janeiro", "Quito", 4805);
    AddDistance("Rio de Janeiro", "Montevideo", 1820);
    AddDistance("Rio de Janeiro", "Asunción", 1833);
    AddDistance("Rio de Janeiro", "La Paz", 3145);
    AddDistance("Rio de Janeiro", "Caracas", 5258);
    AddDistance("Rio de Janeiro", "Guadalajara", 8183);
    AddDistance("Rio de Janeiro", "Medellín", 4639);
    AddDistance("Rio de Janeiro", "Córdoba", 2220);
    AddDistance("Rio de Janeiro", "Mar del Plata", 2831);
    AddDistance("Rio de Janeiro", "Rosario", 2410);
    AddDistance("Rio de Janeiro", "Valparaíso", 3643);

    AddDistance("Belo Horizonte", "Salvador", 1372);
    AddDistance("Belo Horizonte", "Brasília", 716);
    AddDistance("Belo Horizonte", "Curitiba", 1004);
    AddDistance("Belo Horizonte", "Fortaleza", 2528);
    AddDistance("Belo Horizonte", "Recife", 2061);
    AddDistance("Belo Horizonte", "Porto Alegre", 1712);
    AddDistance("Belo Horizonte", "Manaus", 3951);
    AddDistance("Belo Horizonte", "Goiânia", 906);
    AddDistance("Belo Horizonte", "Belém", 2824);
    AddDistance("Belo Horizonte", "Campinas", 589);
    AddDistance("Belo Horizonte", "São Luís", 2738);
    AddDistance("Belo Horizonte", "Maceió", 1854);
    AddDistance("Belo Horizonte", "Natal", 2348);
    AddDistance("Belo Horizonte", "Teresina", 2302);
    AddDistance("Belo Horizonte", "João Pessoa", 2171);

    AddDistance("Salvador", "Brasília", 1446);
    AddDistance("Salvador", "Curitiba", 2385);
    AddDistance("Salvador", "Fortaleza", 1389);
    AddDistance("Salvador", "Recife", 839);
    AddDistance("Salvador", "Porto Alegre", 3090);
    AddDistance("Salvador", "Manaus", 5009);
    AddDistance("Salvador", "Goiânia", 1643);
    AddDistance("Salvador", "Belém", 2100);
    AddDistance("Salvador", "Campinas", 1971);
    AddDistance("Salvador", "São Luís", 1599);
    AddDistance("Salvador", "Maceió", 632);
    AddDistance("Salvador", "Natal", 1126);
    AddDistance("Salvador", "Teresina", 1163);
    AddDistance("Salvador", "João Pessoa", 949);

    AddDistance("Brasília", "Curitiba", 1366);
    AddDistance("Brasília", "Fortaleza", 2200);
    AddDistance("Brasília", "Recife", 2135);
    AddDistance("Brasília", "Porto Alegre", 2027);
    AddDistance("Brasília", "Manaus", 3490);
    AddDistance("Brasília", "Goiânia", 209);
    AddDistance("Brasília", "Belém", 2120);
    AddDistance("Brasília", "Campinas", 873);
    AddDistance("Brasília", "São Luís", 2157);
    AddDistance("Brasília", "Maceió", 1930);
    AddDistance("Brasília", "Natal", 2422);
    AddDistance("Brasília", "Teresina", 1789);
    AddDistance("Brasília", "João Pessoa", 2245);

    AddDistance("Curitiba", "Fortaleza", 3541);
    AddDistance("Curitiba", "Recife", 3078);
    AddDistance("Curitiba", "Porto Alegre", 711);
    AddDistance("Curitiba", "Manaus", 4036);
    AddDistance("Curitiba", "Goiânia", 1186);
    AddDistance("Curitiba", "Belém", 3193);
    AddDistance("Curitiba", "Campinas", 408);
    AddDistance("Curitiba", "São Luís", 3230);
    AddDistance("Curitiba", "Maceió", 2871);
    AddDistance("Curitiba", "Natal", 3162);
    AddDistance("Curitiba", "Teresina", 3143);
    AddDistance("Curitiba", "João Pessoa", 3188);

    AddDistance("Fortaleza", "Recife", 800);
    AddDistance("Fortaleza", "Porto Alegre", 4242);
    AddDistance("Fortaleza", "Manaus", 5763);
    AddDistance("Fortaleza", "Goiânia", 2482);
    AddDistance("Fortaleza", "Belém", 1610);
    AddDistance("Fortaleza", "Campinas", 2805);
    AddDistance("Fortaleza", "São Luís", 1070);
    AddDistance("Fortaleza", "Maceió", 1156);
    AddDistance("Fortaleza", "Natal", 537);
    AddDistance("Fortaleza", "Teresina", 634);
    AddDistance("Fortaleza", "João Pessoa", 688);

    AddDistance("Recife", "Porto Alegre", 3779);
    AddDistance("Recife", "Manaus", 5491);
    AddDistance("Recife", "Goiânia", 2170);
    AddDistance("Recife", "Belém", 2074);
    AddDistance("Recife", "Campinas", 2660);
    AddDistance("Recife", "São Luís", 1573);
    AddDistance("Recife", "Maceió", 285);
    AddDistance("Recife", "Natal", 297);
    AddDistance("Recife", "Teresina", 1137);
    AddDistance("Recife", "João Pessoa", 120);

    AddDistance("Porto Alegre", "Manaus", 4563);
    AddDistance("Porto Alegre", "Goiânia", 1847);
    AddDistance("Porto Alegre", "Belém", 3852);
    AddDistance("Porto Alegre", "Campinas", 1145);
    AddDistance("Porto Alegre", "São Luís", 3891);
    AddDistance("Porto Alegre", "Maceió", 3572);
    AddDistance("Porto Alegre", "Natal", 4066);
    AddDistance("Porto Alegre", "Teresina", 3804);
    AddDistance("Porto Alegre", "João Pessoa", 3889);

    AddDistance("Manaus", "Goiânia", 3291);
    AddDistance("Manaus", "Belém", 5298);
    AddDistance("Manaus", "Campinas", 3909);
    AddDistance("Manaus", "São Luís", 5335);
    AddDistance("Manaus", "Maceió", 5491);
    AddDistance("Manaus", "Natal", 5985);
    AddDistance("Manaus", "Teresina", 5267);
    AddDistance("Manaus", "João Pessoa", 5808);

    AddDistance("Goiânia", "Belém", 2017);
    AddDistance("Goiânia", "Campinas", 810);
    AddDistance("Goiânia", "São Luís", 2054);
    AddDistance("Goiânia", "Maceió", 2105);
    AddDistance("Goiânia", "Natal", 2618);
    AddDistance("Goiânia", "Teresina", 1986);
    AddDistance("Goiânia", "João Pessoa", 2442);

    AddDistance("Belém", "Campinas", 2942);
    AddDistance("Belém", "São Luís", 806);
    AddDistance("Belém", "Maceió", 2173);
    AddDistance("Belém", "Natal", 2108);
    AddDistance("Belém", "Teresina", 947);
    AddDistance("Belém", "João Pessoa", 2161);

    AddDistance("Campinas", "São Luís", 2979);
    AddDistance("Campinas", "Maceió", 2462);
    AddDistance("Campinas", "Natal", 2627);
    AddDistance("Campinas", "Teresina", 2588);
    AddDistance("Campinas", "João Pessoa", 2590);

    AddDistance("São Luís", "Maceió", 1672);
    AddDistance("São Luís", "Natal", 1607);
    AddDistance("São Luís", "Teresina", 446);
    AddDistance("São Luís", "João Pessoa", 1660);

    AddDistance("Maceió", "Natal", 572);
    AddDistance("Maceió", "Teresina", 1236);
    AddDistance("Maceió", "João Pessoa", 395);

    AddDistance("Natal", "Teresina", 1171);
    AddDistance("Natal", "João Pessoa", 185);

    AddDistance("Teresina", "João Pessoa", 1224);
  }

  private void AddDistance(string city1, string city2, double distance) {
    _distances[(city1, city2)] = distance;
    _distances[(city2, city1)] = distance; // Adiciona a distância inversa
  }

  public double GetDistance(string city1, string city2) {
    if (_distances.TryGetValue((city1, city2), out double distance)) {
      return distance;
    }
    return -1; // Retorna -1 se a distância não for encontrada
  }
}