using System;
using System.Collections.Generic;
using System.Text;

namespace Test {
    //Ϊ�˷�������л� �ṩ��ǩ

    [System.Xml.Serialization.XmlRoot()]
    public class XMLTest {
        int a;
        [System.Xml.Serialization.XmlElement()]
        public int A {
            get {
                return a;
            }
            set {
                a = value;
            }
        }

        int b;

        [System.Xml.Serialization.XmlElement()]
        public int B {
            get {
                return b;
            }
            set {
                b = value;
            }
        }

        int c;

        
        int g;
        //���л������Ϊ�ӽڵ��������� ������ȷ     
        [System.Xml.Serialization.XmlAttribute()]
        public int G {
            get {
                return g;
            }
            set {
                g = value;
            }
        }


        [System.Xml.Serialization.XmlElement()]        
        public int C {
            get {
                return c;
            }
            set {
                c = value;
            }
        }

        
        int d;

        [System.Xml.Serialization.XmlElement()]
        public int D {
            get {
                return d;
            }
            set {
                d = value;
            }
        }

        XMLTest2 test2;

        [System.Xml.Serialization.XmlElement()]
        public XMLTest2 Test2 {
            get {
                return test2;
            }
            set {
                test2 = value;
            }
        }
    }

    [System.Xml.Serialization.XmlRoot()]
    public class XMLTest2 {
        int e;

        [System.Xml.Serialization.XmlElement()]
        public int E {
            get {
                return e;
            }
            set {
                e = value;
            }
        }

        int f;

        [System.Xml.Serialization.XmlElement()]
        public int F {
            get {
                return f;
            }
            set {
                f = value;
            }
        }
    }
}
