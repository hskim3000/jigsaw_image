using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;

/// <summary>
/// ����Ͷ�ȡ xml �ļ�
/// </summary>
public class XmlSaver
{
    /// <summary>
    /// ���ݼ���
    /// </summary>
    /// <param name="toE">Ҫ���ܵ��ַ���</param>
    /// <returns>���ܺ���ַ���</returns>
    public string Encrypt(string toE)
    {
        //���ܺͽ��ܲ�����ͬ��key,�����Լ�����Ǳ���Ϊ32λ//
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12348578902223367877723456789012");

        // ���ɶԳƼ��ܹ������
        RijndaelManaged rDel = new RijndaelManaged();

        // ������Կ
        rDel.Key = keyArray;

        // ���ü���ģʽ
        rDel.Mode = CipherMode.ECB;

        // �������ģʽ
        rDel.Padding = PaddingMode.PKCS7;

        // ���ɶԳƼ��ܶ���
        ICryptoTransform cTransform = rDel.CreateEncryptor();

        // �� �ַ��� ����Ϊ utf-8
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toE);

        // ��������
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        // ������ת��Ϊ base64 ����
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }


    /// <summary>
    /// ���ݽ���
    /// </summary>
    /// <param name="toE">Ҫ���ܵ��ַ���</param>
    /// <returns>���ܺ���ַ���</returns>
    public string Decrypt(string toD)
    {
        //���ܺͽ��ܲ�����ͬ��key,����ֵ�Լ�����Ǳ���Ϊ32λ//
        byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12348578902223367877723456789012");

        // ���ɶԳƼ��ܹ������
        RijndaelManaged rDel = new RijndaelManaged();

        // ������Կ
        rDel.Key = keyArray;

        // ���ü���ģʽ
        rDel.Mode = CipherMode.ECB;

        // �������ģʽ
        rDel.Padding = PaddingMode.PKCS7;

        // ���ɶԳƽ��ܶ���
        ICryptoTransform cTransform = rDel.CreateDecryptor();

        // �����ݽ��� base64 ����
        byte[] toEncryptArray = Convert.FromBase64String(toD);

        // ��������
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        // �� utf-8  ����ת�����ַ���
        return UTF8Encoding.UTF8.GetString(resultArray);
    }

    /// <summary>
    /// ���л� ����
    /// </summary>
    /// <param name="pObject">Ҫ���л��Ķ���</param>
    /// <param name="ty">���л��Ķ��������</param>
    /// <returns></returns>
    public string SerializeObject(object pObject, System.Type ty)
    {
        MemoryStream memoryStream = new MemoryStream();

        // ���� xml ���л�����
        XmlSerializer xs = new XmlSerializer(ty);

        // ���� xml Ϊ utf-8
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

        // ���л�����
        xs.Serialize(xmlTextWriter, pObject);

        // ��ȡ���к��������
        memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

        // ���ַ�������Ϊ utf-8 
        return UTF8ByteArrayToString(memoryStream.ToArray());
    }

    /// <summary>
    /// �����л� ����
    /// </summary>
    /// <param name="pXmlizedString">Ҫ�����л����ַ���</param>
    /// <param name="ty">�����л�����������</param>
    /// <returns>�����л������</returns>
    public object DeserializeObject(string pXmlizedString, System.Type ty)
    {
        // ���� xml ���л�����
        XmlSerializer xs = new XmlSerializer(ty);

        // ���ַ���ת�� utf-8 ����
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));

        // ���� xml Ϊ utf-8 ����
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

        // �����л�����
        return xs.Deserialize(memoryStream);
    }

    /// <summary>
    /// ����XML�ļ�
    /// </summary>
    /// <param name="fileName">�ļ���</param>
    /// <param name="thisData">Ҫ������ļ�����</param>
    public void CreateXML(string fileName, string thisData)
    {
        // �����ļ�
        StreamWriter writer = File.CreateText(fileName);

        // �������ݣ���д���ļ�
        writer.Write(Encrypt(thisData));

        // �ر��ļ�
        writer.Close();
    }

    /// <summary>
    /// ��ȡXML�ļ�
    /// </summary>
    /// <param name="fileName">Ҫ��ȡ���ļ���</param>
    /// <returns>�ļ�������</returns>
    public string LoadXML(string fileName)
    {
        // ���ļ�
        StreamReader sReader = File.OpenText(fileName);

        // ��ȡ�ļ�
        string dataString = sReader.ReadToEnd();

        // �ر��ļ�
        sReader.Close();

        // ���ؽ��ܺ������
        return Decrypt(dataString);
    }


    /// <summary>
    /// ��utf-8 ����ת��Ϊ �ַ���
    /// </summary>
    /// <param name="characters">Ҫת����utf-8 ����</param>
    /// <returns>ת���� �� �ַ���</returns>
    public string UTF8ByteArrayToString(byte[] characters)
    {
        // �� utf-8 ���� ת�����ַ���
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);

        // �����ַ���
        return (constructedString);
    }


    /// <summary>
    /// ���ַ���ת��Ϊ utf-8 ����
    /// </summary>
    /// <param name="pXmlString">Ҫת�����ַ���</param>
    /// <returns>ת���� �� utf-8 ����</returns>
    public byte[] StringToUTF8ByteArray(String pXmlString)
    {
        // ���ַ������� utf ����
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);

        // ��������
        return byteArray;
    }
}