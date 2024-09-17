using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimalManager
{
    private static int index;
    private static List<AnimalBehaviour> animals = new List<AnimalBehaviour>();

    public static void AddAnimal(AnimalBehaviour animal)
    {
        animals.Add(animal);
    }
    public static void RemoveAnimal(AnimalBehaviour animal)
    {
        animals.Remove(animal);
    }

    public static void Update()
    {
        if (animals.Count == 0)
            return;
        index++;
        if(index >= animals.Count)
            index = 0;
        animals[index].UpdateBehaviour();
    }

    public static void PlaySound(Vector3 position, float volume)
    {
        foreach (AnimalBehaviour animal in animals)
            animal.PlaySound(position, volume);
    }
}
