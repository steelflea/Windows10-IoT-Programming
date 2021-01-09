#pragma once

#include <opencv2\imgproc.hpp>

namespace ImageProcessingComponent
{
	public enum class ThresholdType
	{
		Binary = cv::ThresholdTypes::THRESH_BINARY,
		BinaryInverted = cv::ThresholdTypes::THRESH_BINARY_INV,
		Trunc = cv::ThresholdTypes::THRESH_TRUNC,
		ToZero = cv::ThresholdTypes::THRESH_TOZERO,
		ToZeroInv = cv::ThresholdTypes::THRESH_TOZERO_INV,
		Otsu = cv::ThresholdTypes::THRESH_OTSU,
		Triangle = cv::ThresholdTypes::THRESH_TRIANGLE
	};
}
