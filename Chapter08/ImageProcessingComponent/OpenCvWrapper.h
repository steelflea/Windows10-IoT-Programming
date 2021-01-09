#pragma once

#include "ThresholdType.h"
#include <opencv2\core.hpp>
#include <opencv2\imgproc.hpp>
#include "ObjectDescriptor.h"

using namespace std;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation::Collections;

namespace ImageProcessingComponent
{
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class OpenCvWrapper sealed
	{
	public:
		static void Threshold(WriteableBitmap^ inputBitmap, int level, ThresholdType type);
		static IVector<ObjectDescriptor^>^ DetectObjects(WriteableBitmap^ inputBitmap, bool drawContours);
		static double Brightness(WriteableBitmap^ inputBitmap);

	private:
		static const int pxMaxValue = 255;

		static void CheckInputParameters(WriteableBitmap^ inputBitmap);

		static cv::Mat ConvertWriteableBitmapToMat(WriteableBitmap^ inputBitmap);
		static cv::Mat ConvertMatToGrayScale(cv::Mat inputMat);

		static byte* GetPointerToPixelBuffer(IBuffer^ pixelBuffer);

		static vector<vector<cv::Point>> FindContours(cv::Mat inputMat);
		static void DrawContours(cv::Mat inputMat, vector<vector<cv::Point>> contours);

		static IVector<ObjectDescriptor^>^ ContoursToObjectList(vector<vector<cv::Point>> contours);
	};
}
