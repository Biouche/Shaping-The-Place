using System.Runtime.InteropServices;
using System;
using UnityEngine;

public class CGALPlugin: MonoBehaviour
{
    [DllImport("cgal-plugin", EntryPoint = "checkIntersection")]
    public static extern int CheckIntersection(IntPtr cutMeshOff, IntPtr cuttedMeshOff);
    [DllImport("cgal-plugin", EntryPoint = "booleanOperation")]
    public static extern IntPtr BooleanOperation(IntPtr offFile1, IntPtr offFile2, IntPtr operationName);
    [DllImport("cgal-plugin", EntryPoint = "booleanOperationClean")]
    public static extern IntPtr BooleanOperationClean(IntPtr offFile1, IntPtr transform1, IntPtr offFile2, IntPtr transform2, IntPtr operationName);
    [DllImport("cgal-plugin", EntryPoint = "booleanOperationQuaternion")]
    public static extern IntPtr BooleanOperationQuaternion(IntPtr cuttedMesh, IntPtr quaternionCuttedMesh, IntPtr cutMesh, IntPtr quaternionCutmesh, IntPtr operationName);

}
