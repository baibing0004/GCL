/*
 * ----------------------------------------------------------------
 * ��Ȩ (C) 2006-2008 
 * �ױ����˱�������Ȩ
 * ��������ҵĿ�ĵ�ʹ��Ȩ����
 * 
 * ����˵����
 * �ױ���д��������
 * 
 * ���ߣ��ױ�
 * 2006-9-22
 * Email��baibing0004@sohu.com 
 * 
 *----------------------------------------------------------------
 */
using System.Reflection;
using System.Runtime.CompilerServices;

//
// �йس��򼯵ĳ�����Ϣ��ͨ������ 
//���Լ����Ƶġ�������Щ����ֵ���޸������
//��������Ϣ��
//
[assembly: AssemblyTitle("�ױ���д��������")]
[assembly: AssemblyDescription("�˰汾���������а����˶�XML�����ļ��Ķ�ȡ���ܡ������˷����������쳣�����ݿ��ѯ���ܡ�Ĭ�ϵ�һЩ�̴߳��������")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("�ڵ¹�˾")]
[assembly: AssemblyProduct("GCL")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// ���򼯵İ汾��Ϣ�������� 4 ��ֵ��
//
//      ���汾
//      �ΰ汾
//      �ڲ��汾��
//      �޶���
//
// ������ָ������ֵ����ʹ�á��޶��š��͡��ڲ��汾�š���Ĭ��ֵ������Ϊ�����·�ʽ 
// ʹ�á�*����
[assembly: AssemblyVersion("3.1.61024.0")]
[assembly: AssemblyFileVersion("3.1.0.0")]
#if(!DEBUG)

#endif

//
// Ҫ�Գ��򼯽���ǩ��������ָ��Ҫʹ�õ���Կ���йس���ǩ���ĸ�����Ϣ����ο� 
// Microsoft .NET ����ĵ���
//
// ʹ����������Կ�������ǩ������Կ��
//
// ע�⣺
//   (*) ���δָ����Կ������򼯲��ᱻǩ����
//   (*) KeyName ��ָ�Ѿ���װ�ڼ�����ϵ�
//      ���ܷ����ṩ���� (CSP) �е���Կ��KeyFile ��ָ����
//       ��Կ���ļ���
//   (*) ��� KeyFile �� KeyName ֵ����ָ������ 
//       �������д���
//       (1) ����� CSP �п����ҵ� KeyName����ʹ�ø���Կ��
//       (2) ��� KeyName �����ڶ� KeyFile ���ڣ��� 
//           KeyFile �е���Կ��װ�� CSP �в���ʹ�ø���Կ��
//   (*) Ҫ���� KeyFile������ʹ�� sn.exe��ǿ���ƣ�ʵ�ù��ߡ�
//       ��ָ�� KeyFile ʱ��KeyFile ��λ��Ӧ�������
//       ��Ŀ���Ŀ¼����
//       %Project Directory%\obj\<configuration>�����磬��� KeyFile λ��
//       ����ĿĿ¼��Ӧ�� AssemblyKeyFile 
//       ����ָ��Ϊ [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) ���ӳ�ǩ������һ���߼�ѡ�� - �й����ĸ�����Ϣ������� Microsoft .NET ���
//       �ĵ���
//
[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile(@"..\..\baibing.snk")]
[assembly: AssemblyKeyName("")]
