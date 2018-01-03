using System.ComponentModel.DataAnnotations;

namespace PInvoke
{
    /// <content>
    /// Contains the Windows Standard Clipboard Formats constants.
    /// </content>
    public partial class User32
    {
        /// <summary>
        /// Contains the Windows Standard Clipboard Formats constants.
        /// Defined in https://msdn.microsoft.com/en-us/library/windows/desktop/ff729168(v=vs.85).aspx
        /// Documentation pulled from MSDN.
        /// </summary>
        public enum StandardClipboardFormat : uint
        {
            /* 
             * Text format. Each line ends with a carriage return/linefeed (CR-LF) combination. 
             * A null character signals the end of the data. Use this format for ANSI text.
             */
            [Display(Name = "CF_TEXT")]
            CF_TEXT = 1,

            /// <summary>
            /// A handle to a bitmap (HBITMAP).
            /// </summary>
            [Display(Name = "CF_BITMAP")]
            CF_BITMAP = 2,

            /* 
             * Handle to a metafile picture format as defined by the METAFILEPICT structure. 
             * When passing a CF_METAFILEPICT handle by means of DDE, the application responsible for deleting 
             * hMem should also free the metafile referred to by the CF_METAFILEPICT handle.
             */
            [Display(Name = "CF_METAFILEPICT")]
            CF_METAFILEPICT = 3,

            // Microsoft Symbolic Link (SYLK) format.
            [Display(Name = "CF_SYLK")]
            CF_SYLK = 4,

            // Software Arts' Data Interchange Format.
            [Display(Name = "CF_DIF")]
            CF_DIF = 5,

            // Tagged-image file format.
            [Display(Name = "CF_TIFF")]
            CF_TIFF = 6,

            /*
             * Text format containing characters in the OEM character set. 
             * Each line ends with a carriage return/linefeed (CR-LF) combination. 
             * A null character signals the end of the data.
             */
            [Display(Name = "CF_OEMTEXT")]
            CF_OEMTEXT = 7,

            /// <summary>
            /// A memory object containing a BITMAPINFO structure followed by the bitmap bits.
            /// </summary>
            [Display(Name = "CF_DIB")]
            CF_DIB = 8,

            /*
             * Handle to a color palette. Whenever an application places data in the clipboard that depends 
             * on or assumes a color palette, it should place the palette on the clipboard as well.
             * 
             * If the clipboard contains data in the CF_PALETTE (logical color palette) format, 
             * the application should use the SelectPalette and RealizePalette functions to realize 
             * (compare) any other data in the clipboard against that logical palette.
             * 
             * When displaying clipboard data, the clipboard always uses as its current palette 
             * any object on the clipboard that is in the CF_PALETTE format.
             */
            [Display(Name = "CF_PALETTE")]
            CF_PALETTE = 9,

            // Data for the pen extensions to the Microsoft Windows for Pen Computing.
            [Display(Name = "CF_PENDATA")]
            CF_PENDATA = 10,

            /*
             * Represents audio data more complex than can be represented in a CF_WAVE standard wave format.
             */
            [Display(Name = "CF_RIFF")]
            CF_RIFF = 11,

            /*
             * Represents audio data in one of the standard wave formats, such as 11 kHz or 22 kHz PCM.
             */
            [Display(Name = "CF_WAVE")]
            CF_WAVE = 12,

            /*
             * Unicode text format. Each line ends with a carriage return/linefeed (CR-LF) 
             * combination. A null character signals the end of the data.
             */
            [Display(Name = "CF_UNICODETEXT")]
            CF_UNICODETEXT = 13,

            // A handle to an enhanced metafile (HENHMETAFILE).
            [Display(Name = "CF_ENHMETAFILE")]
            CF_ENHMETAFILE = 14,

            /*
             * A handle to type HDROP that identifies a list of files. An application can retrieve information 
             * about the files by passing the handle to the DragQueryFile function.
             */
            [Display(Name = "CF_HDROP")]
            CF_HDROP = 15,

            /*
             * The data is a handle to the locale identifier associated with text in the clipboard. When you close the clipboard, 
             * if it contains CF_TEXT data but no CF_LOCALE data, the system automatically sets 
             * the CF_LOCALE format to the current input language. You can use the CF_LOCALE format to associate a 
             * different locale with the clipboard text.
             * 
             * An application that pastes text from the clipboard can retrieve this format to determine which character set 
             * was used to generate the text.
             * 
             * Note that the clipboard does not support plain text in multiple character sets. 
             * To achieve this, use a formatted text data type such as RTF instead.
             * 
             * The system uses the code page associated with CF_LOCALE to implicitly convert from 
             * CF_TEXT to CF_UNICODETEXT. Therefore, the correct code page table is used for the conversion.
             */
            [Display(Name = "CF_LOCALE")]
            CF_LOCALE = 16,

            /// <summary>
            /// A memory object containing a BITMAPV5HEADER structure followed by the bitmap color space information and the bitmap bits.
            /// </summary>
            [Display(Name = "CF_DIBV5")]
            CF_DIBV5 = 17,

            [Display(Name = "CF_MAX")]
            CF_MAX = 18,

            /*
             * Owner-display format. The clipboard owner must display and update the clipboard 
             * viewer window, and receive the WM_ASKCBFORMATNAME, WM_HSCROLLCLIPBOARD, 
             * WM_PAINTCLIPBOARD, WM_SIZECLIPBOARD, and WM_VSCROLLCLIPBOARD messages. 
             * The hMem parameter must be NULL.
             */
            [Display(Name = "CF_OWNERDISPLAY")]
            CF_OWNERDISPLAY = 0x80,

            /* 
             * Text display format associated with a private format. The hMem parameter must be a handle 
             * to data that can be displayed in text format in lieu of the privately formatted data.
             */
            [Display(Name = "CF_DSPTEXT")]
            CF_DSPTEXT = 0x81,

            /*
             *Bitmap display format associated with a private format. The hMem parameter must be a 
             * handle to data that can be displayed in bitmap format in lieu of 
             * the privately formatted data.
             */
            [Display(Name = "CF_DSPBITMAP")]
            CF_DSPBITMAP = 0x82,

            /*
             * Metafile-picture display format associated with a private format. The hMem parameter must 
             * be a handle to data that can be displayed in metafile-picture format in lieu 
             * of the privately formatted data.
             */
            [Display(Name = "CF_DSPMETAFILEPICT")]
            CF_DSPMETAFILEPICT = 0x83, // 

            /*
             * Enhanced metafile display format associated with a private format. 
             * The hMem parameter must be a handle to data that can be displayed in enhanced metafile 
             * format in lieu of the privately formatted data.
             */
            [Display(Name = "CF_DSPENHMETAFILE")]
            CF_DSPENHMETAFILE = 0x8E,

            /*
             * Start of a range of integer values for private clipboard formats. The range ends with CF_PRIVATELAST. 
             * Handles associated with private clipboard formats are not freed automatically; 
             * the clipboard owner must free such handles, typically in response to 
             * the WM_DESTROYCLIPBOARD message.
             */
            [Display(Name = "CF_PRIVATEFIRST")]
            CF_PRIVATEFIRST = 0x0200,

            // See CF_PRIVATEFIRST.
            [Display(Name = "CF_PRIVATELAST")]
            CF_PRIVATELAST = 0x02FF,

            /*
             * Start of a range of integer values for application-defined GDI object clipboard formats. 
             * The end of the range is CF_GDIOBJLAST.
             * 
             * Handles associated with clipboard formats in this range are not automatically deleted using 
             * the GlobalFree function when the clipboard is emptied. Also, when using values in this range, 
             * the hMem parameter is not a handle to a GDI object, but is a handle allocated by the 
             * GlobalAlloc function with the GMEM_MOVEABLE flag.
             */
            [Display(Name = "CF_GDIOBJFIRST")]
            CF_GDIOBJFIRST = 0x0300,

            // See CF_GDIOBJFIRST.
            [Display(Name = "CF_GDIOBJLAST")]
            CF_GDIOBJLAST = 0x03FF,
        }
    }
}