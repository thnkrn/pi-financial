package util

import (
	"crypto/rand"
	"fmt"
	"math/big"
	"regexp"
	"strconv"
)

// IsEmailFormat ตรวจสอบว่า string เป็น email หรือไม่
func IsEmailFormat(email string) bool {
	// ใช้ regex สำหรับตรวจสอบรูปแบบของ email
	emailRegex := `^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`

	// Compile the regex
	re := regexp.MustCompile(emailRegex)

	// ตรวจสอบว่า email ตรงกับ regex หรือไม่
	return re.MatchString(email)
}

// IsCustCodeValid ตรวจสอบว่า custcode เป็นตัวเลขจำนวน 7 หลักหรือไม่
func IsCustCodeValid(custcode string) bool {
	// ใช้ regex สำหรับตรวจสอบรูปแบบของ custcode (ตัวเลข 7 หลัก)
	custCodeRegex := `^\d{7}$`

	// Compile the regex
	re := regexp.MustCompile(custCodeRegex)

	// ตรวจสอบว่า custcode ตรงกับ regex หรือไม่
	return re.MatchString(custcode)
}

func HasSequenceNumbers(input string, maxLength int) bool {
	minLength := maxLength
	matches := []string{}

	// Helper function to check if a string is numeric
	isNumeric := func(str string) bool {
		_, err := strconv.Atoi(str)
		return err == nil
	}

	// Helper function to check if a string is ascending
	isAscending := func(str string) bool {
		for i := 1; i < len(str); i++ {
			if int(str[i])-int(str[i-1]) != 1 {
				return false
			}
		}
		return true
	}

	// Helper function to check if a string has repeating numbers
	isRepeating := func(str string) bool {
		for i := 1; i < len(str); i++ {
			if str[i] != str[i-1] {
				return false
			}
		}
		return true
	}

	// Iterate over the input string to find sequences
	for i := 0; i < len(input); i++ {
		for j := i + minLength; j <= i+maxLength && j <= len(input); j++ {
			sequence := input[i:j]
			if isNumeric(sequence) && (isAscending(sequence) || isRepeating(sequence)) {
				matches = append(matches, sequence)
			}
		}
	}

	return len(matches) > 0
}

// MaskFormattedPhoneNumber รับหมายเลขโทรศัพท์แบบไม่มีขีด (เช่น 0819071886) และส่งคืนหมายเลขที่มีขีดคั่นและซ่อนตัวเลขกลาง (081-XXX-1886)
func MaskFormattedPhoneNumber(phone string) (string, error) {
	// ตรวจสอบว่าเป็นตัวเลข 10 หลัก
	matched, err := regexp.MatchString(`^\d{10}$`, phone)
	if err != nil {
		return "", err
	}
	if !matched {
		return "", fmt.Errorf("invalid phone number format")
	}

	// แปลงจาก 0819071886 เป็น 081-XXX-1886
	formattedPhone := phone[:3] + "-" + "XXX" + "-" + phone[6:]
	return formattedPhone, nil
}

func IsValidUUID(uuid string) bool {
	// regex สำหรับตรวจสอบ UUID รูปแบบมาตรฐาน (ทั้งตัวเล็กและตัวใหญ่)
	regex := `^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$`
	re := regexp.MustCompile(regex)
	return re.MatchString(uuid)
}

// GenerateRandomNumberString generates a string of random digits (0-9) of the given length.
func GenerateRandomNumberString(length int) (*string, error) {
	if length <= 0 {
		return nil, fmt.Errorf("length must be greater than 0")
	}

	const digits = "0123456789"
	digitsLength := int64(len(digits))
	var result string

	for i := 0; i < length; i++ {
		randomIndex, err := rand.Int(rand.Reader, big.NewInt(digitsLength))
		if err != nil {
			return nil, fmt.Errorf("error generating random digit: %v", err)
		}
		result += string(digits[randomIndex.Int64()])
	}
	return &result, nil
}

// GenerateRandomUppercase generates a random string of uppercase English letters with the given length.
func GenerateRandomUppercase(length int) (string, error) {
	if length <= 0 {
		return "", fmt.Errorf("length must be greater than 0")
	}

	const letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
	lettersLength := int64(len(letters))
	result := make([]byte, length)

	for i := 0; i < length; i++ {
		randomIndex, err := rand.Int(rand.Reader, big.NewInt(lettersLength))
		if err != nil {
			return "", fmt.Errorf("error generating random index: %v", err)
		}
		result[i] = letters[randomIndex.Int64()]
	}

	return string(result), nil
}

func SafeStringPtr(s *string) string {
	if s == nil {
		return ""
	}
	return *s
}
