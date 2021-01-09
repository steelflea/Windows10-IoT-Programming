#include "pch.h"
#include "OpenCvWrapper.h"
#include <Robuffer.h>
#include <wrl.h>
#include "ObjectDescriptor.h"

using namespace ImageProcessingComponent;
using namespace Microsoft::WRL;
using namespace Platform;

void OpenCvWrapper::Threshold(WriteableBitmap ^inputBitmap, int thresholdLevel, ThresholdType type)
{
	CheckInputParameters(inputBitmap);

	// Mat 초기화 및 회색조로 변환
	auto inputMat = ConvertWriteableBitmapToMat(inputBitmap);
	auto workingMat = ConvertMatToGrayScale(inputMat);

	// 영상 임계값 처리
	cv::threshold(workingMat, workingMat, thresholdLevel, pxMaxValue, (int) type);

	// Bgra8로 되돌리기
	cv::cvtColor(workingMat, inputMat, CV_GRAY2BGRA);

	// 리소스 반환
	workingMat.release();
}

void OpenCvWrapper::CheckInputParameters(WriteableBitmap^ inputBitmap)
{
	if (inputBitmap == nullptr)
	{
		throw ref new NullReferenceException();
	}
}

cv::Mat OpenCvWrapper::ConvertWriteableBitmapToMat(WriteableBitmap ^inputBitmap)
{
	// 원시 픽셀 데이터에 대한 포인터 획득
	auto imageData = GetPointerToPixelBuffer(inputBitmap->PixelBuffer);

	// OpenCV 이미지 구성
	return cv::Mat(inputBitmap->PixelHeight, inputBitmap->PixelWidth, CV_8UC4, imageData);
}

cv::Mat OpenCvWrapper::ConvertMatToGrayScale(cv::Mat inputMat)
{
	auto workingMat = cv::Mat(inputMat.rows, inputMat.cols, CV_8U);

	cv::cvtColor(inputMat, workingMat, CV_BGRA2GRAY);

	return workingMat;
}

byte* OpenCvWrapper::GetPointerToPixelBuffer(IBuffer^ pixelBuffer)
{
	ComPtr<IBufferByteAccess> bufferByteAccess;

	reinterpret_cast<IInspectable*>(pixelBuffer)->QueryInterface(IID_PPV_ARGS(&bufferByteAccess));

	byte* pixels = nullptr;
	bufferByteAccess->Buffer(&pixels);

	return pixels;
}

IVector<ObjectDescriptor^>^ OpenCvWrapper::DetectObjects(WriteableBitmap^ inputBitmap, bool drawContours)
{
	CheckInputParameters(inputBitmap);

	auto inputMat = ConvertWriteableBitmapToMat(inputBitmap);
	auto workingMat = ConvertMatToGrayScale(inputMat);

	auto contours = FindContours(workingMat);

	if (drawContours)
	{
		DrawContours(inputMat, contours);
	}

	return ContoursToObjectList(contours);
}

IVector<ObjectDescriptor^>^ OpenCvWrapper::ContoursToObjectList(vector<vector<cv::Point>> contours)
{
	Vector<ObjectDescriptor^>^ objectsDetected = ref new Vector<ObjectDescriptor^>();

	const double epsilon = 5;

	for (uint i = 0; i < contours.size(); i++)
	{
		vector<cv::Point> polyLine;

		double contourArea = cv::contourArea(contours.at(i), false);

		if (contourArea > 0)
		{
			cv::approxPolyDP(contours.at(i), polyLine, epsilon, true);

			objectsDetected->Append(ref new ObjectDescriptor(polyLine, contourArea));
		}
	}

	return objectsDetected;
}

double OpenCvWrapper::Brightness(WriteableBitmap^ inputBitmap)
{
	CheckInputParameters(inputBitmap);

	auto inputMat = ConvertWriteableBitmapToMat(inputBitmap);
	auto workingMat = ConvertMatToGrayScale(inputMat);

	return cv::mean(workingMat).val[0];
}

vector<vector<cv::Point>> OpenCvWrapper::FindContours(cv::Mat inputMat)
{
	// 임계값 이미지
	cv::threshold(inputMat, inputMat, 0, pxMaxValue, cv::ThresholdTypes::THRESH_OTSU);

	// 윤곽선 찾기
	vector<vector<cv::Point>> contours;
	cv::findContours(inputMat, contours, cv::RetrievalModes::RETR_LIST,
		cv::ContourApproximationModes::CHAIN_APPROX_SIMPLE);

	return contours;
}

void OpenCvWrapper::DrawContours(cv::Mat inputMat, vector<vector<cv::Point>> contours)
{
	// 선 색상 및 두께
	cv::Scalar red = cv::Scalar(0, 0, 255, 255);
	int thickness = 5;

	// 윤곽선 그리기
	for (uint i = 0; i < contours.size(); i++)
	{
		cv::drawContours(inputMat, contours, i, red, thickness);
	}
}
