﻿using Ray_Tracing.Objects;
using System.Numerics;
using Ray_Tracing;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;
using Ray_Tracing.Materials;
using System.Diagnostics;

namespace Test;

public static partial class Program
{
    private static readonly Size Resolution = new(600, 600);
    private static readonly Scene scene = new();
    private static readonly Camera camera = new();
    private static readonly Quaternion rotationDelta = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1f * (MathF.PI / 180));
    private static readonly Light light = new();
    private static readonly Light light2 = new();
    private static readonly uint[] pixelBuffer = new uint[Resolution.Width * Resolution.Height];

    private static IntPtr hWnd;
    private static IntPtr hdc;

    private static nint WndProc(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        switch (msg)
        {
            case 0x0010: // WM_CLOSE
                PostQuitMessage(0);
                return 0;
        }
        return DefWindowProcW(hWnd, msg, wParam, lParam);
    }

    //Bless DeepSeek for existing. I would feel so bad writing this...
    //Although I still had to fix his mistakes😕😴
    public static void Main()
    {
        #region WindowCreate
        if (!GetModuleHandleExW(0, null, out var phModule))
            throw new Win32Exception(Marshal.GetLastSystemError());

        WNDCLASSEX wndClass = new WNDCLASSEX
        {
            cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
            style = 0x0002 | 0x0001, // CS_HREDRAW | CS_VREDRAW
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProcDelegate)WndProc),
            cbClsExtra = 0,
            cbWndExtra = 0,
            hInstance = phModule,
            hIcon = nint.Zero,
            hCursor = nint.Zero,
            hbrBackground = nint.Zero,
            lpszMenuName = null,
            lpszClassName = "PixelWindowClass",
            hIconSm = nint.Zero
        };

        if (RegisterClassExW(ref wndClass) == 0)
            throw new Win32Exception(Marshal.GetLastSystemError());

        hWnd = CreateWindowExW(
            0,
            "PixelWindowClass",
            "Ray Tracing",
            0x00CF0000 | 0x10000000, // WS_OVERLAPPEDWINDOW | WS_VISIBLE
            100, 100,
            Resolution.Width, Resolution.Height,
            IntPtr.Zero,
            IntPtr.Zero,
            phModule,
            IntPtr.Zero);

        if (hWnd == IntPtr.Zero)
            throw new Win32Exception(Marshal.GetLastSystemError());

        ShowWindow(hWnd, 5); // SW_SHOW
        UpdateWindow(hWnd);
        #endregion

        camera.resolution = Resolution;
        camera.transform.position = new Vector3(0, 2, -10);
        camera.transform.rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, 10 * MathF.PI / 180);
        scene.AddObject(camera);

        light2.transform.position = new Vector3(3, 3, 3);
        light2.Intensity = 3;
        light2.color = Color.IndianRed;
        scene.AddObject(light2);

        light.transform.position = new Vector3(3, 3, -3);
        light.Intensity = 2f;
        scene.AddObject(light);

        MeshRender cube = new();
        cube.material.color = Color.RebeccaPurple;
        cube.mesh = Mesh.Cube;
        scene.AddObject(cube);

        MeshRender plane = new();
        plane.transform.position = new Vector3(0, -0.5f, 0);
        plane.transform.scale *= 5;
        plane.mesh = Mesh.Plane;
        plane.material = new MetallicMaterial() { Roughness = 0.05f, Reflectivity = 0.8f };
        scene.AddObject(plane);

        MeshRender cube2 = new();
        cube2.transform.scale = new Vector3(0.5f);
        cube2.transform.position = new Vector3(1, 0.5f, 0.5f);
        cube2.material.color = Color.Green;
        cube2.mesh = Mesh.Cube;
        scene.AddObject(cube2);

        //MeshRender sphere = new();
        //sphere.transform.position = new Vector3(-1, 0.5f, 0.5f);
        //sphere.material.color = Color.Aqua;
        //sphere.mesh = Mesh.CreateSphere(8, 8);
        //scene.AddObject(sphere);
#if DEBUG
        Stopwatch sw = new Stopwatch();
#endif

        ParallelOptions pO = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

        MSG msg = new();

        //For some reason it behaves strangely, blocks the stream and because of this slows down the rendering.
        //I need to figure it out but I'm going to sleep😴💤💤💤💤
        while (GetMessageW(ref msg, IntPtr.Zero, 0, 0))
        {
#if DEBUG
            sw.Restart();
#endif
            //TranslateMessage(ref msg);
            //DispatchMessageW(ref msg);

            Parallel.For(0, Resolution.Height, pO, y =>
            {
                for (int x = 0; x < Resolution.Width; x++)
                {
                    pixelBuffer[y * Resolution.Width + x] = (uint)camera.GetPixel(x, y).ToArgb();
                }
            });

            /*for (int y = 0; y < Resolution.Height; y++)
                SetPixelsRow(y);*/

            unsafe
            {
                fixed (uint* ptr = pixelBuffer)
                {
#pragma warning disable CA1416
                    using var bitmap = new Bitmap(Resolution.Width, Resolution.Height, Resolution.Width * 4,
                        PixelFormat.Format32bppRgb, (IntPtr)ptr);
                    using var graphics = Graphics.FromHwnd(hWnd);
                    graphics.DrawImageUnscaled(bitmap, 0, 0);
#pragma warning restore CA1416
                }
            }

            //light.transform.Position = Vector3.Transform(light.transform.Position, rotationDelta);
            camera.transform.rotation = Quaternion.Concatenate(camera.transform.rotation, rotationDelta);
            camera.transform.position = Vector3.Transform(camera.transform.position, rotationDelta);

#if DEBUG
            sw.Stop();
            Debug.WriteLine($"Frame time: {sw.ElapsedMilliseconds} ms");
#endif
        }

        UnregisterClassW("PixelWindowClass", phModule);
    }

    #region WinAPI

    private delegate nint WndProcDelegate(nint hWnd, uint msg, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        public nint lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public nint hInstance;
        public nint hIcon;
        public nint hCursor;
        public nint hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string? lpszClassName;
        public nint hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public nint hwnd;
        public uint message;
        public nint wParam;
        public nint lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [LibraryImport("user32")]
    private static partial nint CreateWindowExW(
        uint dwExStyle,
        [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
        [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nint lpParam);

    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32", SetLastError = true)]
    //I fucked this shit Library Import doesn't want to be friends at all
    private static extern ushort RegisterClassExW(ref WNDCLASSEX wndClass);

    [LibraryImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UnregisterClassW([MarshalAs(UnmanagedType.LPWStr)] string lpClassName, nint hInstance);

    [LibraryImport("kernel32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetModuleHandleExW(uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] string? lpModuleName, out nint phModule);

    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UpdateWindow(nint hWnd);

    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetMessageW(ref MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport("user32")]
    private static partial nint DispatchMessageW(ref MSG lpMsg);

    [LibraryImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool TranslateMessage(ref MSG lpMsg);

    [LibraryImport("user32")]
    private static partial nint DefWindowProcW(nint hWnd, uint msg, nint wParam, nint lParam);

    [LibraryImport("user32")]
    private static partial nint PostQuitMessage(int nExitCode);
    #endregion
}

