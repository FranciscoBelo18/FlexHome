using UnityEngine;
using System.Collections.Generic;
using mptcc = Mediapipe.Tasks.Components.Containers;

namespace Mediapipe.Unity
{
  public static class BodyAnalysis
  {
    public static void CalcularAngulo(IReadOnlyList<mptcc.NormalizedLandmark> target, int p1, int p2, int p3)
    {
      if (target == null || target.Count < Mathf.Max(p1, p2, p3))
      {
        Debug.LogError("Pontos insuficientes para calcular ângulo.");
        return;
      }

      // Coordenadas dos pontos
      float x1 = target[p1].x, y1 = target[p1].y, z1 = target[p1].z;
      float x2 = target[p2].x, y2 = target[p2].y, z2 = target[p2].z;
      float x3 = target[p3].x, y3 = target[p3].y, z3 = target[p3].z;

      // Vetores
      float x12 = x2 - x1, y12 = y2 - y1, z12 = z2 - z1;
      float x23 = x3 - x2, y23 = y3 - y2, z23 = z3 - z2;

      // Produto escalar
      float produtoEscalar = (x12 * x23) + (y12 * y23) + (z12 * z23);

      // Módulos dos vetores
      float modulo12 = Mathf.Sqrt((x12 * x12) + (y12 * y12) + (z12 * z12));
      float modulo23 = Mathf.Sqrt((x23 * x23) + (y23 * y23) + (z23 * z23));

      // Cálculo do ângulo
      float angulo = Mathf.Acos(produtoEscalar / (modulo12 * modulo23)) * 180 / Mathf.PI;

      Debug.Log("Ângulo formado entre os pontos " + p1 + ", " + p2 + " e " + p3 + ": " + angulo + " graus");

    }
  }
}
