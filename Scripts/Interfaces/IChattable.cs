using UnityEngine;
using System.Collections;

public interface IChattable<T>
{
    void enableDialogue(T dialogue);
}
