namespace TransportManager.Models;

public class CityDistances {
  private readonly Dictionary<(string, string), double> _distances = [];

  public CityDistances() {
    AddDistance("São Paulo", "Rio de Janeiro", 429);
    AddDistance("São Paulo", "Belo Horizonte", 586);
    AddDistance("São Paulo", "Salvador", 1962);
    AddDistance("São Paulo", "Brasília", 1015);
    AddDistance("São Paulo", "Curitiba", 408);
    AddDistance("São Paulo", "Fortaleza", 2384);
    AddDistance("São Paulo", "Recife", 2660);
    AddDistance("São Paulo", "Porto Alegre", 1109);
    AddDistance("São Paulo", "Manaus", 3890);
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