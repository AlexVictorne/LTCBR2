#include "stdafx.h"
#include <amp.h>
#include <string.h>
#include <string>
#include <amp_math.h>
#include <math.h>
#include <iostream>
#include <cstdlib>
#include <sstream>

using namespace concurrency;
using namespace concurrency::fast_math;
//using namespace concurrency::precise_math;
using std::vector;
using namespace std;
using std::string;

BSTR ANSItoBSTR(const char* input)
{
	BSTR result = NULL;
	int lenA = lstrlenA(input);
	int lenW = ::MultiByteToWideChar(CP_ACP, 0, input, lenA, NULL, 0);
	if (lenW > 0)
	{
		result = ::SysAllocStringLen(0, lenW);
		::MultiByteToWideChar(CP_ACP, 0, input, lenA, result, lenW);
	}

	return result;
}

std::wstring _stdcall AnsiStringToWide(std::string const &Str, UINT CodePage = CP_ACP)
{
	DWORD const BuffSize = MultiByteToWideChar(CodePage, 0, Str.c_str(), -1, NULL, 0);
	if (!BuffSize) return NULL;
	std::vector<wchar_t> Buffer;
	Buffer.resize(BuffSize);
	if (!MultiByteToWideChar(CodePage, 0, Str.c_str(), -1, &Buffer[0], BuffSize)) return NULL;
	return (&Buffer[0]);
}

string GenerateAcceleratorXml(accelerator acc)
{
	wstring description = acc.description;
	wstring device_path_temp = acc.device_path;
	string device_path = string(device_path_temp.begin(), device_path_temp.end());
	int pos = device_path.find("&");
	while (pos > 0)
	{
		device_path.replace(pos, 1, ";");
		pos = device_path.find("&");
	}

	string supports_cpu_shared_memory = acc.supports_cpu_shared_memory ? "true" : "false";
	string supports_double_precision = acc.supports_double_precision ? "true" : "false";
	string supports_limited_double_precision = acc.supports_limited_double_precision ? "true" : "false";
	string has_display = acc.has_display ? "true" : "false";
	string is_debug = acc.is_debug ? "true" : "false";
	string is_emulated = acc.is_emulated ? "true" : "false";
	stringstream ss;
	ss << acc.dedicated_memory;
	string dedicated_memory = ss.str();
	ss << acc.version;
	string version = ss.str();

	return "<Item description=\"" + string(description.begin(), description.end()) + "\"" +
		" devicepath=\"" + device_path + "\"" +
		" supportscpusharedmemory=\"" + supports_cpu_shared_memory + "\"" +
		" supportsdoubleprecision=\"" + supports_double_precision + "\"" +
		" supportslimiteddoubleprecision=\"" + supports_limited_double_precision + "\"" +
		" hasdisplay=\"" + has_display + "\"" +
		" isdebug=\"" + is_debug + "\"" +
		" isemulated=\"" + is_emulated + "\"" +
		" dedicatedmemory=\"" + dedicated_memory + "\"" +
		" version=\"" + version + "\" />";
}

extern "C" __declspec (dllexport) BSTR __stdcall GetAcceleratorsInfo()
{
	string resultStr = "<AcceleratorsDataExchange>";
	std::vector<accelerator> accs = accelerator::get_all();
	for (int i = 0; i < accs.size(); i++) {
		resultStr += GenerateAcceleratorXml(accs[i]);
	}

	resultStr += "</AcceleratorsDataExchange>";
	return ANSItoBSTR(const_cast<char*>(resultStr.c_str()));
}

extern "C" __declspec (dllexport) BOOL __stdcall SetAccelerator(char* devicePath)
{
	return accelerator::set_default(AnsiStringToWide(devicePath));
}

extern "C" __declspec (dllexport) BSTR __stdcall GetCurrentAcceleratorInfo()
{
	accelerator default_acc;
	string resultStr = "<AcceleratorsDataExchange>";
	resultStr += GenerateAcceleratorXml(default_acc);
	resultStr += "</AcceleratorsDataExchange>";
	return ANSItoBSTR(const_cast<char*>(resultStr.c_str()));
}

extern "C" __declspec (dllexport) void __stdcall GraphComputing(
	int _newPartiesCount,
	int _oldPartiesCount,
	int* _computedAttributes,
	int* _newConnections,
	int* _oldConnections,
	int _newConnMaxCount,
	int _oldConnMaxCount,
	int* _result)
{
	concurrency::extent<2> extentAttibutes(_newPartiesCount, _oldPartiesCount);
	concurrency::extent<2> extentNewConnections(_newPartiesCount, _newConnMaxCount);
	concurrency::extent<2> extentOldConnections(_oldPartiesCount, _oldConnMaxCount);

	array_view<int, 2> computedAttributes(extentAttibutes, &_computedAttributes[0]);
	array_view<int, 2> newConnections(extentNewConnections, &_newConnections[0]);
	array_view<int, 2> oldConnections(extentOldConnections, &_oldConnections[0]);

	concurrency::extent<1> extentToParallel(_newPartiesCount);
	concurrency::extent<2> extentToResult(_newPartiesCount, _newConnMaxCount);
	array_view<int, 2> resultOfParallel(extentToResult, &_result[0]);

	parallel_for_each(extentToParallel, [=](index<1> idx) restrict(amp)
	{
		//check this with old
		for (int i = 0; i < _oldPartiesCount; i++)
		{
			int newConnsCount = 0;
			int equalConnsCount = 0;
			if (computedAttributes(idx[0], i) == 1)
			{
				//this connections
				for (int j = 0; j < _newConnMaxCount; j++)
				{
					int newConnection = newConnections(idx[0], j);
					if (newConnection != -1)
					{
						newConnsCount++;
						bool resultSubConn = false;
						//check child with old
						for (int v = 0; v < _oldPartiesCount; v++)
						{
							if (computedAttributes(newConnection, v) == 1)
							{
								for (int m = 0; m < _oldConnMaxCount; m++)
								{
									if (oldConnections(v, m) == i)
									{
										resultSubConn = true;
										equalConnsCount++;
										break;
									}
								}

								if (resultSubConn)
								{
									break;
								}
							}
						}

						if (!resultSubConn)
						{
							resultOfParallel(idx[0], j) = -1;
						}
					}
					else
					{
						if (newConnsCount == equalConnsCount)
						{
							break;
						}
					}
				}
			}
		}
	});

	resultOfParallel.synchronize();
}

extern "C" __declspec (dllexport) void __stdcall HardAttribureComputing(
	int _newPartiesCount,
	int _oldPartiesCount,
	float* _newParties,
	float* _oldParties,
	float* _newPartiesAttrs,
	float* _oldPartiesAttrs,
	int _newPartiesAttrsCount,
	int _oldPartiesAttrsCount,
	int* _result)
{
	//// New parties define.
	concurrency::extent<2> extentNewParties(_newPartiesCount, 3);
	concurrency::extent<3> extentNewPartiesAttrs(_newPartiesCount, _newPartiesAttrsCount, 2);
	array_view<float, 2> newParties(extentNewParties, &_newParties[0]);
	array_view<float, 3> newPartiesAttrs(extentNewPartiesAttrs, &_newPartiesAttrs[0]);

	//// An existing parties define.
	concurrency::extent<2> extentOldParties(_oldPartiesCount, 3);
	concurrency::extent<3> extentOldPartiesAttrs(_oldPartiesCount, _oldPartiesAttrsCount, 2);
	array_view<float, 2> oldParties(extentOldParties, &_oldParties[0]);
	array_view<float, 3> oldPartiesAttrs(extentOldPartiesAttrs, &_oldPartiesAttrs[0]);

	concurrency::extent<2> extentToParallel(_newPartiesCount, _oldPartiesCount);
	array_view<int, 2> resultOfParallel(extentToParallel, &_result[0]);

	parallel_for_each(extentToParallel, [=](index<2> idx) restrict(amp)
	{
		bool result = newParties(idx[0], 1) == oldParties(idx[1], 1) && newParties(idx[0], 2) == oldParties(idx[1], 2);
		if (result)
		{
			for (int i = 0; i < _newPartiesAttrsCount; i++)
			{
				float newPartyAttrName = newPartiesAttrs(idx[0], i, 0);
				float newPartyAttrValue = newPartiesAttrs(idx[0], i, 1);

				if (newPartyAttrName == 0.0f || newPartyAttrValue == 0.0f)
				{
					break;
				}

				for (int j = 0; j < _oldPartiesAttrsCount; j++)
				{
					float oldPartyAttrName = oldPartiesAttrs(idx[1], j, 0);
					float oldPartyAttrValue = oldPartiesAttrs(idx[1], j, 1);

					if (oldPartyAttrName == 0.0f || oldPartyAttrValue == 0.0f)
					{
						break;
					}

					result &= newPartyAttrName == oldPartyAttrName && newPartyAttrValue == oldPartyAttrValue;
					if (result)
					{
						break;
					}
				}

				if (!result)
				{
					break;
				}
			}
		}

		resultOfParallel[idx] = result ? 1 : 0;
	});

	resultOfParallel.synchronize();
}
