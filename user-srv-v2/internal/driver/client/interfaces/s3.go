package interfaces

import (
	"context"
	"time"
)

// S3Client defines the interface for AWS S3 operations
type S3Client interface {
	// GeneratePresignedURL generates a presigned URL for an S3 object
	//
	// Parameters:
	//   - ctx: Context for request cancellation and tracing
	//   - key: The S3 object key (file path in bucket)
	//   - expiration: Duration for which the presigned URL should be valid
	//
	// Returns:
	//   - string: The presigned URL for the S3 object
	//   - error: Error if URL generation fails
	GeneratePresignedURL(ctx context.Context, key string, bucket string, expiration time.Duration) (string, error)
	CopyObject(ctx context.Context, sourceKey string, sourceBucket string, destBucket string) error
}
