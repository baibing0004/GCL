using System;
using System.Collections;
using System.Text;
using System.IO;
using GCL.Common;

namespace GCL.Net {

    /**
     * 主要处理MIME类型的翻译
     * 
     * @author 白冰
     * @version 1.0.60608.1
     */
    public class MIME {
        readonly public static string aab = "application/x-authoware-bin";

        readonly public static string aam = "application/x-authoware-map";

        readonly public static string aas = "application/x-authoware-seg";

        readonly public static string ai = "application/postscript";

        readonly public static string aif = "audio/x-aiff";

        readonly public static string aifc = "audio/x-aiff";

        readonly public static string aiff = "audio/x-aiff";

        readonly public static string als = "audio/X-Alpha5";

        readonly public static string amc = "application/x-mpeg";

        readonly public static string ani = "application/octet-stream";

        readonly public static string asc = "text/plain";

        readonly public static string asd = "application/astound";

        readonly public static string asf = "video/x-ms-asf";

        readonly public static string asn = "application/astound";

        readonly public static string asp = "application/x-asap";

        readonly public static string asx = "video/x-ms-asf";

        readonly public static string au = "audio/basic";

        readonly public static string avb = "application/octet-stream";

        readonly public static string avi = "video/x-msvideo";

        readonly public static string awb = "audio/amr-wb";

        readonly public static string bcpio = "application/x-bcpio";

        readonly public static string bin = "application/octet-stream";

        readonly public static string bld = "application/bld";

        readonly public static string bld2 = "application/bld2";

        readonly public static string bmp = "application/x-MS-bmp";

        readonly public static string bpk = "application/octet-stream";

        readonly public static string bz2 = "application/x-bzip2";

        readonly public static string cal = "image/x-cals";

        readonly public static string ccn = "application/x-cnc";

        readonly public static string cco = "application/x-cocoa";

        readonly public static string cdf = "application/x-netcdf";

        readonly public static string cgi = "magnus-internal/cgi";

        readonly public static string chat = "application/x-chat";

        readonly public static string clp = "application/x-msclip";

        readonly public static string cmx = "application/x-cmx";

        readonly public static string co = "application/x-cult3d-object";

        readonly public static string cod = "image/cis-cod";

        readonly public static string cpio = "application/x-cpio";

        readonly public static string cpt = "application/mac-compactpro";

        readonly public static string crd = "application/x-mscardfile";

        readonly public static string csh = "application/x-csh";

        readonly public static string csv = "text/comma-separated-values";

        readonly public static string csm = "chemical/x-csml";

        readonly public static string csml = "chemical/x-csml";

        readonly public static string css = "text/css";

        readonly public static string cur = "application/octet-stream";

        readonly public static string dcm = "x-lml/x-evm";

        readonly public static string dcr = "application/x-director";

        readonly public static string dcx = "image/x-dcx";

        readonly public static string dhtml = "text/html";

        readonly public static string dir = "application/x-director";

        readonly public static string dll = "application/octet-stream";

        readonly public static string dmg = "application/octet-stream";

        readonly public static string dms = "application/octet-stream";

        readonly public static string doc = "application/msword";

        readonly public static string dot = "application/x-dot";

        readonly public static string dvi = "application/x-dvi";

        readonly public static string dwf = "drawing/x-dwf";

        readonly public static string dwg = "application/x-autocad";

        readonly public static string dxf = "application/x-autocad";

        readonly public static string dxr = "application/x-director";

        readonly public static string ebk = "application/x-expandedbook";

        readonly public static string emb = "chemical/x-embl-dl-nucleotide";

        readonly public static string embl = "chemical/x-embl-dl-nucleotide";

        readonly public static string eps = "application/postscript";

        readonly public static string eri = "image/x-eri";

        readonly public static string es = "audio/echospeech";

        readonly public static string esl = "audio/echospeech";

        readonly public static string etc = "application/x-earthtime";

        readonly public static string etx = "text/x-setext";

        readonly public static string evm = "x-lml/x-evm";

        readonly public static string evy = "application/x-envoy";

        readonly public static string exe = "application/octet-stream";

        readonly public static string fh4 = "image/x-freehand";

        readonly public static string fh5 = "image/x-freehand";

        readonly public static string fhc = "image/x-freehand";

        readonly public static string fif = "image/fif";

        readonly public static string fm = "application/x-maker";

        readonly public static string fpx = "image/x-fpx";

        readonly public static string fvi = "video/isivideo";

        readonly public static string gau = "chemical/x-gaussian-input";

        readonly public static string gca = "application/x-gca-compressed";

        readonly public static string gdb = "x-lml/x-gdb";

        readonly public static string gif = "image/gif";

        readonly public static string gps = "application/x-gps";

        readonly public static string gtar = "application/x-gtar";

        readonly public static string gz = "application/x-gzip";

        readonly public static string hdf = "application/x-hdf";

        readonly public static string hdm = "text/x-hdml";

        readonly public static string hdml = "text/x-hdml";

        readonly public static string hlp = "application/winhlp";

        readonly public static string hqx = "application/mac-binhex40";

        readonly public static string htm = "text/html";

        readonly public static string html = "text/html";

        readonly public static string hts = "text/html";

        readonly public static string ice = "x-conference/x-cooltalk";

        readonly public static string ico = "application/octet-stream";

        readonly public static string ief = "image/ief";

        readonly public static string ifm = "image/gif";

        readonly public static string ifs = "image/ifs";

        readonly public static string imy = "audio/melody";

        readonly public static string ins = "application/x-NET-Install";

        readonly public static string ips = "application/x-ipscript";

        readonly public static string ipx = "application/x-ipix";

        readonly public static string it = "audio/x-mod";

        readonly public static string itz = "audio/x-mod";

        readonly public static string ivr = "i-world/i-vrml";

        readonly public static string j2k = "image/j2k";

        readonly public static string jad = "text/vnd.sun.j2me.app-descriptor";

        readonly public static string jam = "application/x-jam";

        readonly public static string jar = "application/java-archive";

        readonly public static string jnlp = "application/x-java-jnlp-file";

        readonly public static string jpe = "image/jpeg";

        readonly public static string jpeg = "image/jpeg";

        readonly public static string jpg = "image/jpeg";

        readonly public static string jpz = "image/jpeg";

        readonly public static string js = "application/javascript";

        readonly public static string jwc = "application/jwc";

        readonly public static string kjx = "application/x-kjx";

        readonly public static string lak = "x-lml/x-lak";

        readonly public static string latex = "application/x-latex";

        readonly public static string lcc = "application/fastman";

        readonly public static string lcl = "application/x-digitalloca";

        readonly public static string lcr = "application/x-digitalloca";

        readonly public static string lgh = "application/lgh";

        readonly public static string lha = "application/octet-stream";

        readonly public static string lml = "x-lml/x-lml";

        readonly public static string lmlpack = "x-lml/x-lmlpack";

        readonly public static string lsf = "video/x-ms-asf";

        readonly public static string lsx = "video/x-ms-asf";

        readonly public static string lzh = "application/x-lzh";

        readonly public static string m13 = "application/x-msmediaview";

        readonly public static string m14 = "application/x-msmediaview";

        readonly public static string m15 = "audio/x-mod";

        readonly public static string m3u = "audio/x-mpegurl";

        readonly public static string m3url = "audio/x-mpegurl";

        readonly public static string ma1 = "audio/ma1";

        readonly public static string ma2 = "audio/ma2";

        readonly public static string ma3 = "audio/ma3";

        readonly public static string ma5 = "audio/ma5";

        readonly public static string man = "application/x-troff-man";

        readonly public static string map = "magnus-internal/imagemap";

        readonly public static string mbd = "application/mbedlet";

        readonly public static string mct = "application/x-mascot";

        readonly public static string mdb = "application/x-msaccess";

        readonly public static string mdz = "audio/x-mod";

        readonly public static string me = "application/x-troff-me";

        readonly public static string mel = "text/x-vmel";

        readonly public static string mi = "application/x-mif";

        readonly public static string mid = "audio/midi";

        readonly public static string midi = "audio/midi";

        readonly public static string mif = "application/x-mif";

        readonly public static string mil = "image/x-cals";

        readonly public static string mio = "audio/x-mio";

        readonly public static string mmf = "application/x-skt-lbs";

        readonly public static string mng = "video/x-mng";

        readonly public static string mny = "application/x-msmoney";

        readonly public static string moc = "application/x-mocha";

        readonly public static string mocha = "application/x-mocha";

        readonly public static string mod = "audio/x-mod";

        readonly public static string mof = "application/x-yumekara";

        readonly public static string mol = "chemical/x-mdl-molfile";

        readonly public static string mop = "chemical/x-mopac-input";

        readonly public static string mov = "video/quicktime";

        readonly public static string movie = "video/x-sgi-movie";

        readonly public static string mp2 = "audio/x-mpeg";

        readonly public static string mp3 = "audio/x-mpeg";

        readonly public static string mp4 = "video/mp4";

        readonly public static string mpc = "application/vnd.mpohun.certificate";

        readonly public static string mpe = "video/mpeg";

        readonly public static string mpeg = "video/mpeg";

        readonly public static string mpg = "video/mpeg";

        readonly public static string mpg4 = "video/mp4";

        readonly public static string mpga = "audio/mpeg";

        readonly public static string mpn = "application/vnd.mophun.application";

        readonly public static string mpp = "application/vnd.ms-project";

        readonly public static string mps = "application/x-mapserver";

        readonly public static string mrl = "text/x-mrml";

        readonly public static string mrm = "application/x-mrm";

        readonly public static string ms = "application/x-troff-ms";

        readonly public static string mts = "application/metastream";

        readonly public static string mtx = "application/metastream";

        readonly public static string mtz = "application/metastream";

        readonly public static string mzv = "application/metastream";

        readonly public static string nar = "application/zip";

        readonly public static string nbmp = "image/nbmp";

        readonly public static string nc = "application/x-netcdf";

        readonly public static string ndb = "x-lml/x-ndb";

        readonly public static string ndwn = "application/ndwn";

        readonly public static string nif = "application/x-nif";

        readonly public static string nmz = "application/x-scream";

        readonly public static string npx = "application/x-netfpx";

        readonly public static string nsnd = "audio/nsnd";

        readonly public static string nva = "application/x-neva1";

        readonly public static string oda = "application/oda";

        readonly public static string oom = "application/x-AtlasMate-Plugin";

        readonly public static string pac = "audio/x-pac";

        readonly public static string pae = "audio/x-epac";

        readonly public static string pan = "application/x-pan";

        readonly public static string pbm = "image/x-portable-bitmap";

        readonly public static string pcx = "image/x-pcx";

        readonly public static string pda = "image/x-pda";

        readonly public static string pdb = "chemical/x-pdb";

        readonly public static string pdf = "application/pdf";

        readonly public static string pfr = "application/font-tdpfr";

        readonly public static string pgm = "image/x-portable-graymap";

        readonly public static string pict = "image/x-pict";

        readonly public static string pm = "application/x-perl";

        readonly public static string pmd = "application/x-pmd";

        readonly public static string png = "image/png";

        readonly public static string pnm = "image/x-portable-anymap";

        readonly public static string pnz = "image/png";

        readonly public static string pot = "application/vnd.ms-powerpoint";

        readonly public static string ppm = "image/x-portable-pixmap";

        readonly public static string pps = "application/vnd.ms-powerpoint";

        readonly public static string ppt = "application/vnd.ms-powerpoint";

        readonly public static string pqf = "application/x-cprplayer";

        readonly public static string pqi = "application/cprplayer";

        readonly public static string prc = "application/x-prc";

        readonly public static string proxy = "application/x-ns-proxy-autoconfig";

        readonly public static string ps = "application/postscript";

        readonly public static string ptlk = "application/listenup";

        readonly public static string pub = "application/x-mspublisher";

        readonly public static string pvx = "video/x-pv-pvx";

        readonly public static string qcp = "audio/vnd.qcelp";

        readonly public static string qt = "video/quicktime";

        readonly public static string qti = "image/x-quicktime";

        readonly public static string qtif = "image/x-quicktime";

        readonly public static string r3t = "text/vnd.rn-realtext3d";

        readonly public static string ra = "audio/x-pn-realaudio";

        readonly public static string ram = "audio/x-pn-realaudio";

        readonly public static string rar = "application/x-rar-compressed";

        readonly public static string ras = "image/x-cmu-raster";

        readonly public static string rdf = "application/rdf+xml";

        readonly public static string rf = "image/vnd.rn-realflash";

        readonly public static string rgb = "image/x-rgb";

        readonly public static string rlf = "application/x-richlink";

        readonly public static string rm = "audio/x-pn-realaudio";

        readonly public static string rmf = "audio/x-rmf";

        readonly public static string rmm = "audio/x-pn-realaudio";

        readonly public static string rmvb = "audio/x-pn-realaudio";

        readonly public static string rnx = "application/vnd.rn-realplayer";

        readonly public static string roff = "application/x-troff";

        readonly public static string rp = "image/vnd.rn-realpix";

        readonly public static string rpm = "audio/x-pn-realaudio-plugin";

        readonly public static string rt = "text/vnd.rn-realtext";

        readonly public static string rte = "x-lml/x-gps";

        readonly public static string rtf = "application/rtf";

        readonly public static string rtg = "application/metastream";

        readonly public static string rtx = "text/richtext";

        readonly public static string rv = "video/vnd.rn-realvideo";

        readonly public static string rwc = "application/x-rogerwilco";

        readonly public static string s3m = "audio/x-mod";

        readonly public static string s3z = "audio/x-mod";

        readonly public static string sca = "application/x-supercard";

        readonly public static string scd = "application/x-msschedule";

        readonly public static string sdf = "application/e-score";

        readonly public static string sea = "application/x-stuffit";

        readonly public static string sgm = "text/x-sgml";

        readonly public static string sgml = "text/x-sgml";

        readonly public static string sh = "application/x-sh";

        readonly public static string shar = "application/x-shar";

        readonly public static string shtml = "magnus-internal/parsed-html";

        readonly public static string shw = "application/presentations";

        readonly public static string si6 = "image/si6";

        readonly public static string si7 = "image/vnd.stiwap.sis";

        readonly public static string si9 = "image/vnd.lgtwap.sis";

        readonly public static string sis = "application/vnd.symbian.install";

        readonly public static string sit = "application/x-stuffit";

        readonly public static string skd = "application/x-Koan";

        readonly public static string skm = "application/x-Koan";

        readonly public static string skp = "application/x-Koan";

        readonly public static string skt = "application/x-Koan";

        readonly public static string slc = "application/x-salsa";

        readonly public static string smd = "audio/x-smd";

        readonly public static string smi = "application/smil";

        readonly public static string smil = "application/smil";

        readonly public static string smp = "application/studiom";

        readonly public static string smz = "audio/x-smd";

        readonly public static string snd = "audio/basic";

        readonly public static string spc = "text/x-speech";

        readonly public static string spl = "application/futuresplash";

        readonly public static string spr = "application/x-sprite";

        readonly public static string sprite = "application/x-sprite";

        readonly public static string spt = "application/x-spt";

        readonly public static string src = "application/x-wais-source";

        readonly public static string stk = "application/hyperstudio";

        readonly public static string stm = "audio/x-mod";

        readonly public static string sv4cpio = "application/x-sv4cpio";

        readonly public static string sv4crc = "application/x-sv4crc";

        readonly public static string svf = "image/vnd";

        readonly public static string svg = "image/svg-xml";

        readonly public static string svh = "image/svh";

        readonly public static string svr = "x-world/x-svr";

        readonly public static string swf = "application/x-shockwave-flash";

        readonly public static string swfl = "application/x-shockwave-flash";

        readonly public static string t = "application/x-troff";

        readonly public static string tad = "application/octet-stream";

        readonly public static string talk = "text/x-speech";

        readonly public static string tar = "application/x-tar";

        readonly public static string taz = "application/x-tar";

        readonly public static string tbp = "application/x-timbuktu";

        readonly public static string tbt = "application/x-timbuktu";

        readonly public static string tcl = "application/x-tcl";

        readonly public static string tex = "application/x-tex";

        readonly public static string texi = "application/x-texinfo";

        readonly public static string texinfo = "application/x-texinfo";

        readonly public static string tgz = "application/x-tar";

        readonly public static string thm = "application/vnd.eri.thm";

        readonly public static string tif = "image/tiff";

        readonly public static string tiff = "image/tiff";

        readonly public static string tki = "application/x-tkined";

        readonly public static string tkined = "application/x-tkined";

        readonly public static string toc = "application/toc";

        readonly public static string toy = "image/toy";

        readonly public static string tr = "application/x-troff";

        readonly public static string trk = "x-lml/x-gps";

        readonly public static string trm = "application/x-msterminal";

        readonly public static string tsi = "audio/tsplayer";

        readonly public static string tsp = "application/dsptype";

        readonly public static string tsv = "text/tab-separated-values";

        readonly public static string ttf = "application/octet-stream";

        readonly public static string ttz = "application/t-time";

        readonly public static string txt = "text/plain";

        readonly public static string ult = "audio/x-mod";

        readonly public static string ustar = "application/x-ustar";

        readonly public static string uu = "application/x-uuencode";

        readonly public static string uue = "application/x-uuencode";

        readonly public static string vcd = "application/x-cdlink";

        readonly public static string vcf = "text/x-vcard";

        readonly public static string vdo = "video/vdo";

        readonly public static string vib = "audio/vib";

        readonly public static string viv = "video/vivo";

        readonly public static string vivo = "video/vivo";

        readonly public static string vmd = "application/vocaltec-media-desc";

        readonly public static string vmf = "application/vocaltec-media-file";

        readonly public static string vmi = "application/x-dreamcast-vms-info";

        readonly public static string vms = "application/x-dreamcast-vms";

        readonly public static string vox = "audio/voxware";

        readonly public static string vqe = "audio/x-twinvq-plugin";

        readonly public static string vqf = "audio/x-twinvq";

        readonly public static string vql = "audio/x-twinvq";

        readonly public static string vre = "x-world/x-vream";

        readonly public static string vrml = "x-world/x-vrml";

        readonly public static string vrt = "x-world/x-vrt";

        readonly public static string vrw = "x-world/x-vream";

        readonly public static string vts = "workbook/formulaone";

        readonly public static string wav = "audio/x-wav";

        readonly public static string wax = "audio/x-ms-wax";

        readonly public static string wbmp = "image/vnd.wap.wbmp";

        readonly public static string web = "application/vnd.xara";

        readonly public static string wi = "image/wavelet";

        readonly public static string wis = "application/x-InstallShield";

        readonly public static string wm = "video/x-ms-wm";

        readonly public static string wma = "audio/x-ms-wma";

        readonly public static string wmd = "application/x-ms-wmd";

        readonly public static string wmf = "application/x-msmetafile";

        readonly public static string wml = "text/vnd.wap.wml";

        readonly public static string wmlc = "application/vnd.wap.wmlc";

        readonly public static string wmls = "text/vnd.wap.wmlscript";

        readonly public static string wmlsc = "application/vnd.wap.wmlscriptc";

        readonly public static string wmlscript = "text/vnd.wap.wmlscript";

        readonly public static string wmv = "audio/x-ms-wmv";

        readonly public static string wmx = "video/x-ms-wmx";

        readonly public static string wmz = "application/x-ms-wmz";

        readonly public static string wpng = "image/x-up-wpng";

        readonly public static string wpt = "x-lml/x-gps";

        readonly public static string wri = "application/x-mswrite";

        readonly public static string wrl = "x-world/x-vrml";

        readonly public static string wrz = "x-world/x-vrml";

        readonly public static string ws = "text/vnd.wap.wmlscript";

        readonly public static string wsc = "application/vnd.wap.wmlscriptc";

        readonly public static string wv = "video/wavelet";

        readonly public static string wvx = "video/x-ms-wvx";

        readonly public static string wxl = "application/x-wxl";

        readonly public static string xar = "application/vnd.xara";

        readonly public static string xbm = "image/x-xbitmap";

        readonly public static string xdm = "application/x-xdma";

        readonly public static string xdma = "application/x-xdma";

        readonly public static string xdw = "application/vnd.fujixerox.docuworks";

        readonly public static string xht = "application/xhtml+xml";

        readonly public static string xhtm = "application/xhtml+xml";

        readonly public static string xhtml = "application/xhtml+xml";

        readonly public static string xla = "application/vnd.ms-excel";

        readonly public static string xlc = "application/vnd.ms-excel";

        readonly public static string xll = "application/x-excel";

        readonly public static string xlm = "application/vnd.ms-excel";

        readonly public static string xls = "application/vnd.ms-excel";

        readonly public static string xlt = "application/vnd.ms-excel";

        readonly public static string xlw = "application/vnd.ms-excel";

        readonly public static string xm = "audio/x-mod";

        readonly public static string xml = "text/xml";

        readonly public static string xmz = "audio/x-mod";

        readonly public static string xpi = "application/x-xpinstall";

        readonly public static string xpm = "image/x-xpixmap";

        readonly public static string xsit = "text/xml";

        readonly public static string xsl = "text/xml";

        readonly public static string xul = "text/xul";

        readonly public static string xwd = "image/x-xwindowdump";

        readonly public static string xyz = "chemical/x-pdb";

        readonly public static string yz1 = "application/x-yz1";

        readonly public static string z = "application/x-compress";

        readonly public static string zac = "application/x-zaurus-zac";

        readonly public static string post = "application/x-www-form-urlencoded";

        private static IDictionary _map = null;
        private static object _mapKey = DateTime.Now;
        public MIME() {
        }

        static MIME() {
            mime();
        }

        protected static void mime() {
            lock (_mapKey) {
                if (_map == null || _map.Count == 0) {
                    _map = new Hashtable();
                    _map.Add("123", "application/vnd.lotus-1-2-3");
                    _map.Add("3gp", "video/3gpp");
                    _map.Add("aab", "application/x-authoware-bin");
                    _map.Add("aam", "application/x-authoware-map");
                    _map.Add("aas", "application/x-authoware-seg");
                    _map.Add("ai", "application/postscript");
                    _map.Add("aif", "audio/x-aiff");
                    _map.Add("aifc", "audio/x-aiff");
                    _map.Add("aiff", "audio/x-aiff");
                    _map.Add("als", "audio/X-Alpha5");
                    _map.Add("amc", "application/x-mpeg");
                    _map.Add("ani", "application/octet-stream");
                    _map.Add("asc", "text/plain");
                    _map.Add("asd", "application/astound");
                    _map.Add("asf", "video/x-ms-asf");
                    _map.Add("asn", "application/astound");
                    _map.Add("asp", "application/x-asap");
                    _map.Add("asx", "video/x-ms-asf");
                    _map.Add("au", "audio/basic");
                    _map.Add("avb", "application/octet-stream");
                    _map.Add("avi", "video/x-msvideo");
                    _map.Add("awb", "audio/amr-wb");
                    _map.Add("bcpio", "application/x-bcpio");
                    _map.Add("bin", "application/octet-stream");
                    _map.Add("bld", "application/bld");
                    _map.Add("bld2", "application/bld2");
                    _map.Add("bmp", "application/x-MS-bmp");
                    _map.Add("bpk", "application/octet-stream");
                    _map.Add("bz2", "application/x-bzip2");
                    _map.Add("cal", "image/x-cals");
                    _map.Add("ccn", "application/x-cnc");
                    _map.Add("cco", "application/x-cocoa");
                    _map.Add("cdf", "application/x-netcdf");
                    _map.Add("cgi", "magnus-internal/cgi");
                    _map.Add("chat", "application/x-chat");
                    _map.Add("class", "application/octet-stream");
                    _map.Add("clp", "application/x-msclip");
                    _map.Add("cmx", "application/x-cmx");
                    _map.Add("co", "application/x-cult3d-object");
                    _map.Add("cod", "image/cis-cod");
                    _map.Add("cpio", "application/x-cpio");
                    _map.Add("cpt", "application/mac-compactpro");
                    _map.Add("crd", "application/x-mscardfile");
                    _map.Add("csh", "application/x-csh");
                    _map.Add("csv", "text/comma-separated-values");
                    _map.Add("csm", "chemical/x-csml");
                    _map.Add("csml", "chemical/x-csml");
                    _map.Add("css", "text/css");
                    _map.Add("cur", "application/octet-stream");
                    _map.Add("dcm", "x-lml/x-evm");
                    _map.Add("dcr", "application/x-director");
                    _map.Add("dcx", "image/x-dcx");
                    _map.Add("dhtml", "text/html");
                    _map.Add("dir", "application/x-director");
                    _map.Add("dll", "application/octet-stream");
                    _map.Add("dmg", "application/octet-stream");
                    _map.Add("dms", "application/octet-stream");
                    _map.Add("doc", "application/msword");
                    _map.Add("dot", "application/x-dot");
                    _map.Add("dvi", "application/x-dvi");
                    _map.Add("dwf", "drawing/x-dwf");
                    _map.Add("dwg", "application/x-autocad");
                    _map.Add("dxf", "application/x-autocad");
                    _map.Add("dxr", "application/x-director");
                    _map.Add("ebk", "application/x-expandedbook");
                    _map.Add("emb", "chemical/x-embl-dl-nucleotide");
                    _map.Add("embl", "chemical/x-embl-dl-nucleotide");
                    _map.Add("eps", "application/postscript");
                    _map.Add("eri", "image/x-eri");
                    _map.Add("es", "audio/echospeech");
                    _map.Add("esl", "audio/echospeech");
                    _map.Add("etc", "application/x-earthtime");
                    _map.Add("etx", "text/x-setext");
                    _map.Add("evm", "x-lml/x-evm");
                    _map.Add("evy", "application/x-envoy");
                    _map.Add("exe", "application/octet-stream");
                    _map.Add("fh4", "image/x-freehand");
                    _map.Add("fh5", "image/x-freehand");
                    _map.Add("fhc", "image/x-freehand");
                    _map.Add("fif", "image/fif");
                    _map.Add("fm", "application/x-maker");
                    _map.Add("fpx", "image/x-fpx");
                    _map.Add("fvi", "video/isivideo");
                    _map.Add("gau", "chemical/x-gaussian-input");
                    _map.Add("gca", "application/x-gca-compressed");
                    _map.Add("gdb", "x-lml/x-gdb");
                    _map.Add("gif", "image/gif");
                    _map.Add("gps", "application/x-gps");
                    _map.Add("gtar", "application/x-gtar");
                    _map.Add("gz", "application/x-gzip");
                    _map.Add("hdf", "application/x-hdf");
                    _map.Add("hdm", "text/x-hdml");
                    _map.Add("hdml", "text/x-hdml");
                    _map.Add("hlp", "application/winhlp");
                    _map.Add("hqx", "application/mac-binhex40");
                    _map.Add("htm", "text/html");
                    _map.Add("html", "text/html");
                    _map.Add("hts", "text/html");
                    _map.Add("ice", "x-conference/x-cooltalk");
                    _map.Add("ico", "application/octet-stream");
                    _map.Add("ief", "image/ief");
                    _map.Add("ifm", "image/gif");
                    _map.Add("ifs", "image/ifs");
                    _map.Add("imy", "audio/melody");
                    _map.Add("ins", "application/x-NET-Install");
                    _map.Add("ips", "application/x-ipscript");
                    _map.Add("ipx", "application/x-ipix");
                    _map.Add("it", "audio/x-mod");
                    _map.Add("itz", "audio/x-mod");
                    _map.Add("ivr", "i-world/i-vrml");
                    _map.Add("j2k", "image/j2k");
                    _map.Add("jad", "text/vnd.sun.j2me.app-descriptor");
                    _map.Add("jam", "application/x-jam");
                    _map.Add("jar", "application/java-archive");
                    _map.Add("jnlp", "application/x-java-jnlp-file");
                    _map.Add("jpe", "image/jpeg");
                    _map.Add("jpeg", "image/jpeg");
                    _map.Add("jpg", "image/jpeg");
                    _map.Add("jpz", "image/jpeg");
                    _map.Add("js", "application/javascript");
                    _map.Add("jwc", "application/jwc");
                    _map.Add("kjx", "application/x-kjx");
                    _map.Add("lak", "x-lml/x-lak");
                    _map.Add("latex", "application/x-latex");
                    _map.Add("lcc", "application/fastman");
                    _map.Add("lcl", "application/x-digitalloca");
                    _map.Add("lcr", "application/x-digitalloca");
                    _map.Add("lgh", "application/lgh");
                    _map.Add("lha", "application/octet-stream");
                    _map.Add("lml", "x-lml/x-lml");
                    _map.Add("lmlpack", "x-lml/x-lmlpack");
                    _map.Add("lsf", "video/x-ms-asf");
                    _map.Add("lsx", "video/x-ms-asf");
                    _map.Add("lzh", "application/x-lzh");
                    _map.Add("m13", "application/x-msmediaview");
                    _map.Add("m14", "application/x-msmediaview");
                    _map.Add("m15", "audio/x-mod");
                    _map.Add("m3u", "audio/x-mpegurl");
                    _map.Add("m3url", "audio/x-mpegurl");
                    _map.Add("ma1", "audio/ma1");
                    _map.Add("ma2", "audio/ma2");
                    _map.Add("ma3", "audio/ma3");
                    _map.Add("ma5", "audio/ma5");
                    _map.Add("man", "application/x-troff-man");
                    _map.Add("map", "magnus-internal/imagemap");
                    _map.Add("mbd", "application/mbedlet");
                    _map.Add("mct", "application/x-mascot");
                    _map.Add("mdb", "application/x-msaccess");
                    _map.Add("mdz", "audio/x-mod");
                    _map.Add("me", "application/x-troff-me");
                    _map.Add("mel", "text/x-vmel");
                    _map.Add("mi", "application/x-mif");
                    _map.Add("mid", "audio/midi");
                    _map.Add("midi", "audio/midi");
                    _map.Add("mif", "application/x-mif");
                    _map.Add("mil", "image/x-cals");
                    _map.Add("mio", "audio/x-mio");
                    _map.Add("mmf", "application/x-skt-lbs");
                    _map.Add("mng", "video/x-mng");
                    _map.Add("mny", "application/x-msmoney");
                    _map.Add("moc", "application/x-mocha");
                    _map.Add("mocha", "application/x-mocha");
                    _map.Add("mod", "audio/x-mod");
                    _map.Add("mof", "application/x-yumekara");
                    _map.Add("mol", "chemical/x-mdl-molfile");
                    _map.Add("mop", "chemical/x-mopac-input");
                    _map.Add("mov", "video/quicktime");
                    _map.Add("movie", "video/x-sgi-movie");
                    _map.Add("mp2", "audio/x-mpeg");
                    _map.Add("mp3", "audio/x-mpeg");
                    _map.Add("mp4", "video/mp4");
                    _map.Add("mpc", "application/vnd.mpohun.certificate");
                    _map.Add("mpe", "video/mpeg");
                    _map.Add("mpeg", "video/mpeg");
                    _map.Add("mpg", "video/mpeg");
                    _map.Add("mpg4", "video/mp4");
                    _map.Add("mpga", "audio/mpeg");
                    _map.Add("mpn", "application/vnd.mophun.application");
                    _map.Add("mpp", "application/vnd.ms-project");
                    _map.Add("mps", "application/x-mapserver");
                    _map.Add("mrl", "text/x-mrml");
                    _map.Add("mrm", "application/x-mrm");
                    _map.Add("ms", "application/x-troff-ms");
                    _map.Add("mts", "application/metastream");
                    _map.Add("mtx", "application/metastream");
                    _map.Add("mtz", "application/metastream");
                    _map.Add("mzv", "application/metastream");
                    _map.Add("nar", "application/zip");
                    _map.Add("nbmp", "image/nbmp");
                    _map.Add("nc", "application/x-netcdf");
                    _map.Add("ndb", "x-lml/x-ndb");
                    _map.Add("ndwn", "application/ndwn");
                    _map.Add("nif", "application/x-nif");
                    _map.Add("nmz", "application/x-scream");
                    _map.Add("nokia-op-logo", "image/vnd.nok-oplogo-color");
                    _map.Add("npx", "application/x-netfpx");
                    _map.Add("nsnd", "audio/nsnd");
                    _map.Add("nva", "application/x-neva1");
                    _map.Add("oda", "application/oda");
                    _map.Add("oom", "application/x-AtlasMate-Plugin");
                    _map.Add("pac", "audio/x-pac");
                    _map.Add("pae", "audio/x-epac");
                    _map.Add("pan", "application/x-pan");
                    _map.Add("pbm", "image/x-portable-bitmap");
                    _map.Add("pcx", "image/x-pcx");
                    _map.Add("pda", "image/x-pda");
                    _map.Add("pdb", "chemical/x-pdb");
                    _map.Add("pdf", "application/pdf");
                    _map.Add("pfr", "application/font-tdpfr");
                    _map.Add("pgm", "image/x-portable-graymap");
                    _map.Add("pict", "image/x-pict");
                    _map.Add("pm", "application/x-perl");
                    _map.Add("pmd", "application/x-pmd");
                    _map.Add("png", "image/png");
                    _map.Add("pnm", "image/x-portable-anymap");
                    _map.Add("pnz", "image/png");
                    _map.Add("pot", "application/vnd.ms-powerpoint");
                    _map.Add("ppm", "image/x-portable-pixmap");
                    _map.Add("pps", "application/vnd.ms-powerpoint");
                    _map.Add("ppt", "application/vnd.ms-powerpoint");
                    _map.Add("pqf", "application/x-cprplayer");
                    _map.Add("pqi", "application/cprplayer");
                    _map.Add("prc", "application/x-prc");
                    _map.Add("proxy", "application/x-ns-proxy-autoconfig");
                    _map.Add("ps", "application/postscript");
                    _map.Add("ptlk", "application/listenup");
                    _map.Add("pub", "application/x-mspublisher");
                    _map.Add("pvx", "video/x-pv-pvx");
                    _map.Add("qcp", "audio/vnd.qcelp");
                    _map.Add("qt", "video/quicktime");
                    _map.Add("qti", "image/x-quicktime");
                    _map.Add("qtif", "image/x-quicktime");
                    _map.Add("r3t", "text/vnd.rn-realtext3d");
                    _map.Add("ra", "audio/x-pn-realaudio");
                    _map.Add("ram", "audio/x-pn-realaudio");
                    _map.Add("rar", "application/x-rar-compressed");
                    _map.Add("ras", "image/x-cmu-raster");
                    _map.Add("rdf", "application/rdf+xml");
                    _map.Add("rf", "image/vnd.rn-realflash");
                    _map.Add("rgb", "image/x-rgb");
                    _map.Add("rlf", "application/x-richlink");
                    _map.Add("rm", "audio/x-pn-realaudio");
                    _map.Add("rmf", "audio/x-rmf");
                    _map.Add("rmm", "audio/x-pn-realaudio");
                    _map.Add("rmvb", "audio/x-pn-realaudio");
                    _map.Add("rnx", "application/vnd.rn-realplayer");
                    _map.Add("roff", "application/x-troff");
                    _map.Add("rp", "image/vnd.rn-realpix");
                    _map.Add("rpm", "audio/x-pn-realaudio-plugin");
                    _map.Add("rt", "text/vnd.rn-realtext");
                    _map.Add("rte", "x-lml/x-gps");
                    _map.Add("rtf", "application/rtf");
                    _map.Add("rtg", "application/metastream");
                    _map.Add("rtx", "text/richtext");
                    _map.Add("rv", "video/vnd.rn-realvideo");
                    _map.Add("rwc", "application/x-rogerwilco");
                    _map.Add("s3m", "audio/x-mod");
                    _map.Add("s3z", "audio/x-mod");
                    _map.Add("sca", "application/x-supercard");
                    _map.Add("scd", "application/x-msschedule");
                    _map.Add("sdf", "application/e-score");
                    _map.Add("sea", "application/x-stuffit");
                    _map.Add("sgm", "text/x-sgml");
                    _map.Add("sgml", "text/x-sgml");
                    _map.Add("sh", "application/x-sh");
                    _map.Add("shar", "application/x-shar");
                    _map.Add("shtml", "magnus-internal/parsed-html");
                    _map.Add("shw", "application/presentations");
                    _map.Add("si6", "image/si6");
                    _map.Add("si7", "image/vnd.stiwap.sis");
                    _map.Add("si9", "image/vnd.lgtwap.sis");
                    _map.Add("sis", "application/vnd.symbian.install");
                    _map.Add("sit", "application/x-stuffit");
                    _map.Add("skd", "application/x-Koan");
                    _map.Add("skm", "application/x-Koan");
                    _map.Add("skp", "application/x-Koan");
                    _map.Add("skt", "application/x-Koan");
                    _map.Add("slc", "application/x-salsa");
                    _map.Add("smd", "audio/x-smd");
                    _map.Add("smi", "application/smil");
                    _map.Add("smil", "application/smil");
                    _map.Add("smp", "application/studiom");
                    _map.Add("smz", "audio/x-smd");
                    _map.Add("snd", "audio/basic");
                    _map.Add("spc", "text/x-speech");
                    _map.Add("spl", "application/futuresplash");
                    _map.Add("spr", "application/x-sprite");
                    _map.Add("sprite", "application/x-sprite");
                    _map.Add("spt", "application/x-spt");
                    _map.Add("src", "application/x-wais-source");
                    _map.Add("stk", "application/hyperstudio");
                    _map.Add("stm", "audio/x-mod");
                    _map.Add("sv4cpio", "application/x-sv4cpio");
                    _map.Add("sv4crc", "application/x-sv4crc");
                    _map.Add("svf", "image/vnd");
                    _map.Add("svg", "image/svg-xml");
                    _map.Add("svh", "image/svh");
                    _map.Add("svr", "x-world/x-svr");
                    _map.Add("swf", "application/x-shockwave-flash");
                    _map.Add("swfl", "application/x-shockwave-flash");
                    _map.Add("t", "application/x-troff");
                    _map.Add("tad", "application/octet-stream");
                    _map.Add("talk", "text/x-speech");
                    _map.Add("tar", "application/x-tar");
                    _map.Add("taz", "application/x-tar");
                    _map.Add("tbp", "application/x-timbuktu");
                    _map.Add("tbt", "application/x-timbuktu");
                    _map.Add("tcl", "application/x-tcl");
                    _map.Add("tex", "application/x-tex");
                    _map.Add("texi", "application/x-texinfo");
                    _map.Add("texinfo", "application/x-texinfo");
                    _map.Add("tgz", "application/x-tar");
                    _map.Add("thm", "application/vnd.eri.thm");
                    _map.Add("tif", "image/tiff");
                    _map.Add("tiff", "image/tiff");
                    _map.Add("tki", "application/x-tkined");
                    _map.Add("tkined", "application/x-tkined");
                    _map.Add("toc", "application/toc");
                    _map.Add("toy", "image/toy");
                    _map.Add("tr", "application/x-troff");
                    _map.Add("trk", "x-lml/x-gps");
                    _map.Add("trm", "application/x-msterminal");
                    _map.Add("tsi", "audio/tsplayer");
                    _map.Add("tsp", "application/dsptype");
                    _map.Add("tsv", "text/tab-separated-values");
                    _map.Add("ttf", "application/octet-stream");
                    _map.Add("ttz", "application/t-time");
                    _map.Add("txt", "text/plain");
                    _map.Add("ult", "audio/x-mod");
                    _map.Add("ustar", "application/x-ustar");
                    _map.Add("uu", "application/x-uuencode");
                    _map.Add("uue", "application/x-uuencode");
                    _map.Add("vcd", "application/x-cdlink");
                    _map.Add("vcf", "text/x-vcard");
                    _map.Add("vdo", "video/vdo");
                    _map.Add("vib", "audio/vib");
                    _map.Add("viv", "video/vivo");
                    _map.Add("vivo", "video/vivo");
                    _map.Add("vmd", "application/vocaltec-media-desc");
                    _map.Add("vmf", "application/vocaltec-media-file");
                    _map.Add("vmi", "application/x-dreamcast-vms-info");
                    _map.Add("vms", "application/x-dreamcast-vms");
                    _map.Add("vox", "audio/voxware");
                    _map.Add("vqe", "audio/x-twinvq-plugin");
                    _map.Add("vqf", "audio/x-twinvq");
                    _map.Add("vql", "audio/x-twinvq");
                    _map.Add("vre", "x-world/x-vream");
                    _map.Add("vrml", "x-world/x-vrml");
                    _map.Add("vrt", "x-world/x-vrt");
                    _map.Add("vrw", "x-world/x-vream");
                    _map.Add("vts", "workbook/formulaone");
                    _map.Add("wav", "audio/x-wav");
                    _map.Add("wax", "audio/x-ms-wax");
                    _map.Add("wbmp", "image/vnd.wap.wbmp");
                    _map.Add("web", "application/vnd.xara");
                    _map.Add("wi", "image/wavelet");
                    _map.Add("wis", "application/x-InstallShield");
                    _map.Add("wm", "video/x-ms-wm");
                    _map.Add("wma", "audio/x-ms-wma");
                    _map.Add("wmd", "application/x-ms-wmd");
                    _map.Add("wmf", "application/x-msmetafile");
                    _map.Add("wml", "text/vnd.wap.wml");
                    _map.Add("wmlc", "application/vnd.wap.wmlc");
                    _map.Add("wmls", "text/vnd.wap.wmlscript");
                    _map.Add("wmlsc", "application/vnd.wap.wmlscriptc");
                    _map.Add("wmlscript", "text/vnd.wap.wmlscript");
                    _map.Add("wmv", "audio/x-ms-wmv");
                    _map.Add("wmx", "video/x-ms-wmx");
                    _map.Add("wmz", "application/x-ms-wmz");
                    _map.Add("wpng", "image/x-up-wpng");
                    _map.Add("wpt", "x-lml/x-gps");
                    _map.Add("wri", "application/x-mswrite");
                    _map.Add("wrl", "x-world/x-vrml");
                    _map.Add("wrz", "x-world/x-vrml");
                    _map.Add("ws", "text/vnd.wap.wmlscript");
                    _map.Add("wsc", "application/vnd.wap.wmlscriptc");
                    _map.Add("wv", "video/wavelet");
                    _map.Add("wvx", "video/x-ms-wvx");
                    _map.Add("wxl", "application/x-wxl");
                    _map.Add("x-gzip", "application/x-gzip");
                    _map.Add("xar", "application/vnd.xara");
                    _map.Add("xbm", "image/x-xbitmap");
                    _map.Add("xdm", "application/x-xdma");
                    _map.Add("xdma", "application/x-xdma");
                    _map.Add("xdw", "application/vnd.fujixerox.docuworks");
                    _map.Add("xht", "application/xhtml+xml");
                    _map.Add("xhtm", "application/xhtml+xml");
                    _map.Add("xhtml", "application/xhtml+xml");
                    _map.Add("xla", "application/vnd.ms-excel");
                    _map.Add("xlc", "application/vnd.ms-excel");
                    _map.Add("xll", "application/x-excel");
                    _map.Add("xlm", "application/vnd.ms-excel");
                    _map.Add("xls", "application/vnd.ms-excel");
                    _map.Add("xlt", "application/vnd.ms-excel");
                    _map.Add("xlw", "application/vnd.ms-excel");
                    _map.Add("xm", "audio/x-mod");
                    _map.Add("xml", "text/xml");
                    _map.Add("xmz", "audio/x-mod");
                    _map.Add("xpi", "application/x-xpinstall");
                    _map.Add("xpm", "image/x-xpixmap");
                    _map.Add("xsit", "text/xml");
                    _map.Add("xsl", "text/xml");
                    _map.Add("xul", "text/xul");
                    _map.Add("xwd", "image/x-xwindowdump");
                    _map.Add("xyz", "chemical/x-pdb");
                    _map.Add("yz1", "application/x-yz1");
                    _map.Add("z", "application/x-compress");
                    _map.Add("zac", "application/x-zaurus-zac");

                    _map.Add("post", "application/x-www-form-urlencoded");
                }
            }
        }

        /**
         * 根据文件的类型获取对应的MIME解释 没有找到时为空
         * 
         * @param url
         *            文件路径
         * @return
         */
        public static string GetType(string url) {
            try {
                return Tool.ToStringValue(_map[new FileInfo(url).Extension.ToLower().TrimStart('.')]);
            } catch {
                return "";
            }
        }

        /// <summary>
        /// 复制一个键值对字典
        /// </summary>
        /// <returns></returns>
        public static IDictionary CloneMap() {
            IDictionary newmap = new Hashtable();
            IO.IOTool.Transport(_map, newmap);
            return newmap;
        }
    }
}
