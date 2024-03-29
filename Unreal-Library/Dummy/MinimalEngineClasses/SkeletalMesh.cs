﻿using System.Collections.Generic;

namespace UELib.Dummy
{
    internal class SkeletalMesh : MinimalBase
    {
        //Calling this minimal is just a plain lie..
        protected override byte[] MinimalByteArray { get; } =
        {
            0x00, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x43, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x0A, 0xD7, 0xA3, 0x3C, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x7C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x1E, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1D, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1A,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x41,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3D, 0x00, 0x00, 0x00, 0x2E, 0x2E, 0x5C, 0x2E, 0x2E,
            0x5C, 0x2E, 0x2E, 0x5C, 0x2E, 0x2E, 0x5C, 0x55, 0x73, 0x65, 0x72, 0x73, 0x5C, 0x4D, 0x61, 0x72,
            0x74, 0x69, 0x6E, 0x5C, 0x4F, 0x6E, 0x65, 0x44, 0x72, 0x69, 0x76, 0x65, 0x5C, 0x52, 0x4C, 0x5C,
            0x4D, 0x69, 0x6E, 0x69, 0x6D, 0x61, 0x6C, 0x53, 0x6B, 0x65, 0x6C, 0x65, 0x74, 0x61, 0x6C, 0x4D,
            0x65, 0x73, 0x68, 0x2E, 0x66, 0x62, 0x78, 0x00, 0x1B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x14, 0x00, 0x00, 0x00, 0x32, 0x30, 0x31, 0x39, 0x2D, 0x30, 0x36, 0x2D, 0x32, 0x39, 0x20, 0x31,
            0x32, 0x3A, 0x32, 0x35, 0x3A, 0x33, 0x30, 0x00, 0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80,
            0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
            0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF3, 0x04, 0x35, 0x3F,
            0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0xF3, 0x04, 0x35, 0xBF, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x2F, 0x31, 0x0D, 0xA4, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0xBF,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0xBF, 0x30, 0x31, 0x8D, 0xA4, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x03, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00,
            0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x02, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x0C, 0x00,
            0x0D, 0x00, 0x0E, 0x00, 0x0D, 0x00, 0x0F, 0x00, 0x10, 0x00, 0x15, 0x00, 0x16, 0x00, 0x17, 0x00,
            0x16, 0x00, 0x15, 0x00, 0x18, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x05, 0x00, 0x04, 0x00, 0x07, 0x00, 0x04, 0x00, 0x05, 0x00, 0x06, 0x00, 0x12, 0x00,
            0x11, 0x00, 0x14, 0x00, 0x11, 0x00, 0x12, 0x00, 0x13, 0x00, 0x09, 0x00, 0x08, 0x00, 0x0B, 0x00,
            0x08, 0x00, 0x09, 0x00, 0x0A, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x19, 0x00, 0x00, 0x00, 0x01, 0x00, 0x80, 0xBF, 0x00, 0x00, 0x80, 0x3F,
            0xFD, 0xFF, 0x7F, 0xBF, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0x00, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x80, 0x3F, 0xFC, 0xFF, 0x7F, 0xBF, 0x01, 0x00, 0x80,
            0xBF, 0x7F, 0xFF, 0x7F, 0x80, 0x7F, 0x00, 0x7F, 0x80, 0x7F, 0x7F, 0x00, 0x80, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF,
            0xFF, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x80, 0x3F, 0xFD, 0xFF, 0x7F, 0xBF, 0x7F, 0x00,
            0x7F, 0x80, 0xF1, 0xB8, 0x7F, 0x80, 0x7F, 0x7F, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFA,
            0xFF, 0x7F, 0xBF, 0x02, 0x00, 0x80, 0xBF, 0x01, 0x00, 0x80, 0xBF, 0x7F, 0x7F, 0x7F, 0x80, 0x25,
            0x25, 0x7F, 0x80, 0x7F, 0x7F, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0x7F, 0xBF,
            0x01, 0x00, 0x80, 0xBF, 0xFD, 0xFF, 0x7F, 0x3F, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0x7F, 0x80,
            0x7F, 0x7F, 0xFF, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xF5, 0xFF, 0x7F, 0x3F, 0x04, 0x00, 0x80,
            0x3F, 0x01, 0x00, 0x80, 0x3F, 0x7F, 0x00, 0x7F, 0x80, 0x7F, 0xFE, 0x7F, 0x80, 0x7F, 0x7F, 0xFF,
            0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x04, 0x00, 0x80, 0x3F, 0xFA, 0xFF, 0x7F, 0xBF, 0xFD, 0xFF,
            0x7F, 0x3F, 0x7F, 0xFF, 0x7F, 0x80, 0xF1, 0x46, 0x7F, 0x80, 0x7F, 0x7F, 0xFF, 0x80, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF,
            0xFF, 0xFF, 0x00, 0x03, 0x00, 0x80, 0xBF, 0xF7, 0xFF, 0x7F, 0x3F, 0x01, 0x00, 0x80, 0x3F, 0x7F,
            0x7F, 0x7F, 0x80, 0x25, 0xD9, 0x7F, 0x80, 0x7F, 0x7F, 0xFF, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00,
            0x04, 0x00, 0x80, 0x3F, 0xFA, 0xFF, 0x7F, 0xBF, 0xFD, 0xFF, 0x7F, 0x3F, 0x7F, 0xD9, 0xD9, 0x80,
            0x7F, 0x25, 0xD9, 0x80, 0xFF, 0x7F, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x80,
            0x3F, 0x01, 0x00, 0x80, 0x3F, 0xFD, 0xFF, 0x7F, 0xBF, 0x7F, 0x25, 0x25, 0x80, 0x7F, 0xD9, 0x25,
            0x80, 0xFF, 0x7F, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x80, 0x3F, 0xFC, 0xFF,
            0x7F, 0xBF, 0x01, 0x00, 0x80, 0xBF, 0x7F, 0xD9, 0x25, 0x80, 0x7F, 0x25, 0x25, 0x80, 0xFF, 0x7F,
            0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xF5, 0xFF, 0x7F, 0x3F, 0x04, 0x00, 0x80, 0x3F, 0x01,
            0x00, 0x80, 0x3F, 0x7F, 0x25, 0xD9, 0x80, 0x7F, 0xD9, 0xD9, 0x80, 0xFF, 0x7F, 0x7F, 0x80, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF,
            0xFF, 0xFF, 0xFF, 0x00, 0xF5, 0xFF, 0x7F, 0x3F, 0x04, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x80, 0x3F,
            0x7F, 0x7F, 0xFF, 0x80, 0x7F, 0x7F, 0xFE, 0x80, 0x7F, 0xFF, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
            0x00, 0x01, 0x00, 0x80, 0xBF, 0x00, 0x00, 0x80, 0x3F, 0xFD, 0xFF, 0x7F, 0xBF, 0x7F, 0x7F, 0x7F,
            0x80, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0xFF, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00,
            0x80, 0x3F, 0x01, 0x00, 0x80, 0x3F, 0xFD, 0xFF, 0x7F, 0xBF, 0x7F, 0x7F, 0x00, 0x80, 0xF1, 0x7F,
            0x46, 0x80, 0x7F, 0xFF, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xF5, 0xFF, 0x7F, 0x3F, 0x04,
            0x00, 0x80, 0x3F, 0x01, 0x00, 0x80, 0x3F, 0x7F, 0x7F, 0xFE, 0x80, 0x7F, 0x7F, 0xFF, 0x80, 0x7F,
            0xFF, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x03, 0x00, 0x80, 0xBF, 0xF7, 0xFF, 0x7F, 0x3F,
            0x01, 0x00, 0x80, 0x3F, 0x7F, 0x7F, 0x7F, 0x80, 0x25, 0x7F, 0xD9, 0x80, 0x7F, 0xFF, 0x7F, 0x80,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0x7F, 0xBF, 0x01, 0x00, 0x80, 0xBF, 0xFD, 0xFF, 0x7F,
            0x3F, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0x7F, 0x80, 0x00, 0x7F, 0x7F, 0x80, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF,
            0xFF, 0x00, 0x01, 0x00, 0x80, 0xBF, 0x00, 0x00, 0x80, 0x3F, 0xFD, 0xFF, 0x7F, 0xBF, 0x7F, 0x7F,
            0x7F, 0x80, 0x7F, 0x7F, 0x7F, 0x80, 0x00, 0x7F, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x03,
            0x00, 0x80, 0xBF, 0xF7, 0xFF, 0x7F, 0x3F, 0x01, 0x00, 0x80, 0x3F, 0x7F, 0x7F, 0x7F, 0x80, 0x7F,
            0xD9, 0xD9, 0x80, 0x00, 0x7F, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFA, 0xFF, 0x7F, 0xBF,
            0x02, 0x00, 0x80, 0xBF, 0x01, 0x00, 0x80, 0xBF, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x25, 0x25, 0x80,
            0x00, 0x7F, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0x7F, 0xBF, 0x01, 0x00, 0x80,
            0xBF, 0xFD, 0xFF, 0x7F, 0x3F, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x00, 0x7F,
            0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x80, 0x3F, 0xFC, 0xFF, 0x7F, 0xBF, 0x01, 0x00,
            0x80, 0xBF, 0x7F, 0x7F, 0x00, 0x80, 0x7F, 0x7F, 0x00, 0x80, 0x7F, 0x00, 0x7F, 0x80, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF,
            0xFF, 0xFF, 0x00, 0xFA, 0xFF, 0x7F, 0xBF, 0x02, 0x00, 0x80, 0xBF, 0x01, 0x00, 0x80, 0xBF, 0x7F,
            0x7F, 0x7F, 0x80, 0x25, 0x7F, 0x25, 0x80, 0x7F, 0x00, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00,
            0x04, 0x00, 0x80, 0x3F, 0xFA, 0xFF, 0x7F, 0xBF, 0xFD, 0xFF, 0x7F, 0x3F, 0x7F, 0x7F, 0xFF, 0x80,
            0xF1, 0x7F, 0xB8, 0x80, 0x7F, 0x00, 0x7F, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x19, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x02, 0x00, 0x00, 0x00, 0x00, 0x19, 0x00, 0x00, 0x00, 0x64, 0x00, 0x00, 0x00, 0xA7, 0x0F,
            0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x00,
            0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x06, 0x00,
            0x00, 0x00, 0x16, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x1A, 0x00,
            0x00, 0x00, 0x1B, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x1C, 0x00,
            0x00, 0x00, 0x1D, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x1E, 0x00,
            0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x22, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x15, 0x00,
            0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00,
            0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00,
            0x00, 0x00, 0x19, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0x00, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x01, 0x00, 0x80, 0xBF, 0x00, 0x00, 0x80, 0x3F, 0xFD, 0xFF,
            0x7F, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0xFF, 0x7F, 0x80, 0x7F, 0x7F, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0xFC, 0xFF, 0x7F, 0xBF, 0x01, 0x00,
            0x80, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x00, 0x7F, 0x80, 0x7F, 0x7F, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x80, 0x3F, 0xFD, 0xFF,
            0x7F, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFA, 0xFF, 0x7F, 0xBF, 0x02, 0x00, 0x80, 0xBF, 0x01, 0x00,
            0x80, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0xFF, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0xBF, 0x01, 0x00, 0x80, 0xBF, 0xFD, 0xFF,
            0x7F, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x00, 0x7F, 0x80, 0x7F, 0x7F, 0xFF, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x7F, 0x3F, 0x04, 0x00, 0x80, 0x3F, 0x01, 0x00,
            0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0xFF, 0x7F, 0x80, 0x7F, 0x7F, 0xFF, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x04, 0x00, 0x80, 0x3F, 0xFA, 0xFF, 0x7F, 0xBF, 0xFD, 0xFF,
            0x7F, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x7F, 0xFF, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x03, 0x00, 0x80, 0xBF, 0xF7, 0xFF, 0x7F, 0x3F, 0x01, 0x00,
            0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0xD9, 0xD9, 0x80, 0xFF, 0x7F, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x04, 0x00, 0x80, 0x3F, 0xFA, 0xFF, 0x7F, 0xBF, 0xFD, 0xFF,
            0x7F, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x25, 0x25, 0x80, 0xFF, 0x7F, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x80, 0x3F, 0xFD, 0xFF,
            0x7F, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0xD9, 0x25, 0x80, 0xFF, 0x7F, 0x7F, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0xFC, 0xFF, 0x7F, 0xBF, 0x01, 0x00,
            0x80, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x25, 0xD9, 0x80, 0xFF, 0x7F, 0x7F, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x7F, 0x3F, 0x04, 0x00, 0x80, 0x3F, 0x01, 0x00,
            0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0xFF, 0x80, 0x7F, 0xFF, 0x7F, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x7F, 0x3F, 0x04, 0x00, 0x80, 0x3F, 0x01, 0x00,
            0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0xFF, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x01, 0x00, 0x80, 0xBF, 0x00, 0x00, 0x80, 0x3F, 0xFD, 0xFF,
            0x7F, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x00, 0x80, 0x7F, 0xFF, 0x7F, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x01, 0x00, 0x80, 0x3F, 0xFD, 0xFF,
            0x7F, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0xFE, 0x80, 0x7F, 0xFF, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xF5, 0xFF, 0x7F, 0x3F, 0x04, 0x00, 0x80, 0x3F, 0x01, 0x00,
            0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0xFF, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x03, 0x00, 0x80, 0xBF, 0xF7, 0xFF, 0x7F, 0x3F, 0x01, 0x00,
            0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x00, 0x7F, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0xBF, 0x01, 0x00, 0x80, 0xBF, 0xFD, 0xFF,
            0x7F, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x00, 0x7F, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x01, 0x00, 0x80, 0xBF, 0x00, 0x00, 0x80, 0x3F, 0xFD, 0xFF,
            0x7F, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x00, 0x7F, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x03, 0x00, 0x80, 0xBF, 0xF7, 0xFF, 0x7F, 0x3F, 0x01, 0x00,
            0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x00, 0x7F, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFA, 0xFF, 0x7F, 0xBF, 0x02, 0x00, 0x80, 0xBF, 0x01, 0x00,
            0x80, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x00, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x7F, 0xBF, 0x01, 0x00, 0x80, 0xBF, 0xFD, 0xFF,
            0x7F, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x00, 0x80, 0x7F, 0x00, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0xFC, 0xFF, 0x7F, 0xBF, 0x01, 0x00,
            0x80, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0x7F, 0x80, 0x7F, 0x00, 0x7F, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFA, 0xFF, 0x7F, 0xBF, 0x02, 0x00, 0x80, 0xBF, 0x01, 0x00,
            0x80, 0xBF, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x7F, 0xFF, 0x80, 0x7F, 0x00, 0x7F, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x04, 0x00, 0x80, 0x3F, 0xFA, 0xFF, 0x7F, 0xBF, 0xFD, 0xFF,
            0x7F, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x02,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public SkeletalMesh(UExportTableItem exportTableItem, UnrealPackage package) : base(exportTableItem, package)
        {
            FixNameIndexAtPosition(package, "ClothingAssets", 4);
            FixNameIndexAtPosition(package, "ArrayProperty", 12);
            FixNameIndexAtPosition(package, "LODInfo", 36);
            FixNameIndexAtPosition(package, "ArrayProperty", 44);
            FixNameIndexAtPosition(package, "DisplayFactor", 64);
            FixNameIndexAtPosition(package, "FloatProperty", 72);
            FixNameIndexAtPosition(package, "LODHysteresis", 92);
            FixNameIndexAtPosition(package, "FloatProperty", 100);
            FixNameIndexAtPosition(package, "LODMaterialMap", 120);
            FixNameIndexAtPosition(package, "ArrayProperty", 128);
            FixNameIndexAtPosition(package, "bEnableShadowCasting", 148);
            FixNameIndexAtPosition(package, "ArrayProperty", 156);
            FixNameIndexAtPosition(package, "TriangleSortSettings", 177);
            FixNameIndexAtPosition(package, "ArrayProperty", 185);
            FixNameIndexAtPosition(package, "TriangleSorting", 205);
            FixNameIndexAtPosition(package, "ByteProperty", 213);
            FixNameIndexAtPosition(package, "TriangleSortOption", 229);
            FixNameIndexAtPosition(package, "TRISORT_None", 237);
            FixNameIndexAtPosition(package, "CustomLeftRightAxis", 245);
            FixNameIndexAtPosition(package, "ByteProperty", 253);
            FixNameIndexAtPosition(package, "TriangleSortAxis", 269);
            FixNameIndexAtPosition(package, "TSA_X_Axis", 277);
            FixNameIndexAtPosition(package, "CustomLeftRightBoneName", 285);
            FixNameIndexAtPosition(package, "NameProperty", 293);
            FixNameIndexAtPosition(package, "None", 309);
            FixNameIndexAtPosition(package, "None", 317);
            FixNameIndexAtPosition(package, "bDisableCompressions", 325);
            FixNameIndexAtPosition(package, "BoolProperty", 333);
            FixNameIndexAtPosition(package, "bHasBeenSimplified", 350);
            FixNameIndexAtPosition(package, "BoolProperty", 358);
            FixNameIndexAtPosition(package, "None", 375);
            FixNameIndexAtPosition(package, "SourceFilePath", 383);
            FixNameIndexAtPosition(package, "StrProperty", 391);
            FixNameIndexAtPosition(package, "SourceFileTimestamp", 472);
            FixNameIndexAtPosition(package, "StrProperty", 480);
            FixNameIndexAtPosition(package, "None", 520);
        }

        protected override void WriteSerialData(IUnrealStream stream, UnrealPackage package)
        {
            stream.Write(MinimalByteArray, 0, MinimalByteArray.Length);
        }

        public static void AddNamesToNameTable(UnrealPackage package)
        {
            var namesToAdd = new List<string>()
            {
                "ClothingAssets", "ArrayProperty", "LODInfo", "SourceFilePath",
                "StrProperty", "SourceFileTimestamp", "DisplayFactor", "FloatProperty",
                "LODHysteresis", "LODMaterialMap", "bEnableShadowCasting", "TriangleSortSettings",
                "TriangleSorting", "ByteProperty", "TriangleSortOption", "TRISORT_None", "CustomLeftRightAxis",
                "TriangleSortAxis", "TSA_X_Axis", "CustomLeftRightBoneName", "NameProperty",
                "bDisableCompressions", "BoolProperty", "bHasBeenSimplified",
            };
            AddNamesToNameTable(package, namesToAdd);
        }
    }
}