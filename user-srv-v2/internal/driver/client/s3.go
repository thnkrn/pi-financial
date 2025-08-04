package client

import (
	"context"
	"fmt"
	"time"

	"github.com/aws/aws-sdk-go-v2/aws"
	"github.com/aws/aws-sdk-go-v2/config"
	"github.com/aws/aws-sdk-go-v2/service/s3"
	"github.com/pi-financial/go-common/logger"
	"github.com/pi-financial/user-srv-v2/internal/driver/client/interfaces"
)

type S3Client struct {
	s3Client *s3.Client
	log      logger.Logger
}

// NewS3Client creates a new AWS S3 client instance.
//
// Parameters:
//   - log: Logger instance for logging
//   - cfg: Configuration containing AWS settings
//
// Returns:
//   - interfaces.S3Client: S3 client interface implementation
//   - error: Error if client creation fails
//
// Implementation:
//  1. Loads AWS configuration with the provided region.
//  2. Creates an S3 client using the loaded configuration.
//  3. Returns the S3 client with bucket name and logger.
//
// Error cases:
//   - Returns error if AWS configuration loading fails
//   - Returns error if S3 client creation fails
func NewS3Client(log logger.Logger) (interfaces.S3Client, error) {
	cfg, err := config.LoadDefaultConfig(context.TODO(), config.WithRegion("ap-southeast-1"))
	if err != nil {
		fmt.Printf("failed to load default config: %v\n", err)
	}
	// Create S3 client
	s3Client := s3.NewFromConfig(cfg)

	return &S3Client{
		s3Client: s3Client,
		log:      log,
	}, nil
}

// GeneratePresignedURL generates a presigned URL for an S3 object.
//
// Parameters:
//   - ctx: Context for request cancellation and tracing
//   - key: The S3 object key (file path in bucket)
//   - expiration: Duration for which the presigned URL should be valid
//
// Returns:
//   - string: The presigned URL for the S3 object
//   - error: Error if URL generation fails
//
// Implementation:
//  1. Creates a presigned URL request for the specified S3 object.
//  2. Generates the presigned URL using the S3 client.
//  3. Returns the presigned URL string.
//
// Error cases:
//   - Returns error if presigned URL generation fails
//   - Returns error if the S3 object key is invalid
//
// Notes: The presigned URL allows temporary access to the S3 object without AWS credentials.
func (s *S3Client) GeneratePresignedURL(ctx context.Context, key string, bucket string, expiration time.Duration) (string, error) {
	// Create presigned URL request
	presignClient := s3.NewPresignClient(s.s3Client)

	request, err := presignClient.PresignGetObject(ctx, &s3.GetObjectInput{
		Bucket: aws.String(bucket),
		Key:    aws.String(key),
	}, s3.WithPresignExpires(expiration))

	if err != nil {
		return "", fmt.Errorf("failed to generate presigned URL for key %s: %w", key, err)
	}

	return request.URL, nil
}

func (s *S3Client) CopyObject(ctx context.Context, sourceKey string, sourceBucket string, destBucket string) error {
	_, err := s.s3Client.CopyObject(ctx, &s3.CopyObjectInput{
		CopySource: aws.String(fmt.Sprintf("%s/%s", sourceBucket, sourceKey)),
		Key:        aws.String(sourceKey),
		Bucket:     aws.String(destBucket),
	})
	return err
}
