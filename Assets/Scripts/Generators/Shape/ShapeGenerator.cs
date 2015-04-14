using UnityEngine;
using System.Collections;

public interface ShapeGenerator {

    Shape Generate();
    Shape.ConnectionTime GetConnectionTime();
    void SetEntrance();
}
