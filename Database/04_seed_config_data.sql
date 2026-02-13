/*
==================================================================================
Library Management System - Complete Initial data seeding Script
==================================================================================
This script creates:
- All lookup tables data and create initial user(1 Admin, 1 Librarian and 1 User)
==================================================================================
*/

USE [LibraryManagementSys]
GO

SET IDENTITY_INSERT [dbo].[StatusType] ON 
GO

INSERT [dbo].[StatusType] ([Id], [Label], [Description], [IsActive]) VALUES 
	(1, N'Book', N'Status related to book availability, condition, or categorization.', 1),
	(2, N'Transaction', N'Status related to borrowing, returning, or renewing books.', 1),
	(3, N'Fine', N'Status related to overdue fines, payments, and penalties.', 1),
	(4, N'Reservations', N'Status related to reserving the book.', 1)
GO

SET IDENTITY_INSERT [dbo].[StatusType] OFF
GO

SET IDENTITY_INSERT [dbo].[Status] ON 
GO

INSERT [dbo].[Status] ([Id], [StatusTypeId], [Label], [Description], [Color], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, 1, N'Available', N'Book is in the library and available for borrowing.', N'#28A745', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(2, 1, N'CheckedOut', N'Book is checked out by a user.', N'#007BFF', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(3, 1, N'Reserved', N'Book is reserved by a user but not yet picked up or issued.', N'#FD7E14', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(4, 1, N'Lost_Damaged', N'Book is lost or damaged and cannot be borrowed until fixed or replaced.', N'#DC3545', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(5, 1, N'OnHold', N'Book is temporarily unavailable for any reason.', N'#FFC107', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(6, 1, N'Removed', N'Book is removed from library wait to collect all copy from user', N'#ff8800', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(7, 2, N'Returned', N'Book is returned to the library.', N'#28A745', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(8, 2, N'Overdue', N'User failed to return the book by the due date.', N'#B71C1C', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(9, 2, N'Renewed', N'Book has been renewed by the user for an extended period.', N'#6F42C1', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(10, 2, N'Cancelled', N'Borrowing process was cancelled.', N'#6C757D', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(11, 2, N'Borrowed', N'Book is checked out to the user.', N'#007BFF', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(12, 2, N'ClaimedLost', N'The borrower has reported the book loss.', N'#FFA500', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(13, 3, N'Paid', N'Penalty was paid.', N'#28A745', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(14, 3, N'UnPaid', N'Penalty not paid.', N'#DC3545', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(15, 3, N'Waived', N'penalty is canceled or not applied due to valid reasons', N'#6C757D', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(16, 4, N'Fulfilled', N'The reservation is complete, and the book has been borrowed by the user.', N'#007BFF', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(17, 4, N'Reserved', N'User has reserved the book but hasn’t picked it up yet.', N'#FD7E14', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(18, 4, N'Cancelled', N'User has cancelled the reservation.', N'#6C757D', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(19, 4, N'Allocated', N'Book has been allocated', N'#20C997', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[Status] OFF
GO

SET IDENTITY_INSERT [dbo].[Configs] ON 
GO

INSERT [dbo].[Configs] ([Id], [KeyName], [KeyValue], [Description], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, N'MaxActiveMembership', N'2', N'Maximum number of memberships that a user is allowed to purchase.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(2, N'NextPlanActivationTimeInMinutes', N'5', N'If you purchase a second plan prior to the expiration of your current plan, there will be a designated period before the new plan becomes active', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(3, N'ProfilePhotoDirectoryPath', N'assets/Uploads/ProfilePhotos/Leatest', N'Storage place in wwwroot for profile photos which is currently in used', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(4, N'ProfilePhotoArchiveDirectoryPath', N'assets/Uploads/ProfilePhotos/Archive', N'Storage place in wwwroot for profile photos which was removed from the account.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(5, N'BookCoverPageDirectoryPath', N'assets/Uploads/CoverPages/Leatest', N'Storage place in wwwroot for cover pages which is currently in used for the book.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(6, N'BookCoverPageArchiveDirectoryPath', N'assets/Uploads/ProfilePhotos/Archive', N'Storage place in wwwroot for profile photos which was removed from the account.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(7, N'BookPreviewDirectoryPath', N'assets/Uploads/BookPreview/Leatest', N'Storage place in wwwroot for book preview which is currently in used for book details', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(8, N'BookPreviewArchiveDirectoryPath', N'assets/Uploads/BookPreview/Archive', N'Storage place in wwwroot for book preview which was removed from the book details', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(9, N'ImageFileExtensions', N'.png, .jpeg, .jpg', N'Valid file format for image file', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(10, N'BorrowDueDays', N'15', N'Max number of days till which you need to return/remew the book', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(11, N'AllocationDueDays', N'3', N'Max number of days till which book is allocated to your reservation after that your reservation will be cancled', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(12, N'BasePenaltyPerDay', N'10', N'Starting Penalty from day for late Return / Renew', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(13, N'PenaltyIncreaseType', N'+', N'Choose like whether to add or multiply PenaltyIncreaseValue in BasePenaltyPerDay', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(14, N'PenaltyIncreaseValue', N'5', N' Value to update based on PenaltyIncreaseType in BasePenaltyPerDay after PenaltyIncreaseDurationInDay', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(15, N'PenaltyIncreaseDurationInDays', N'5', N'Duration in days after which penalty will update based on PenaltyIncreaseType', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(16, N'DefaultAllocationDelayInDays', N'2', N'Default allocation delay after refused your allocation in days', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(17, N'MembershipExpiryBufferDays', N'2', N'This buffer allows users extra time in days to renew their membership without immediate consequences, such as late fees or access limitations.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(18, N'PreviousLimitCarryoverDays', N'1', N'This buffer period ensures a smooth transition, giving users time in days to return excess books without immediate penalties or restrictions.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(19, N'MaxTransferAllocationCount', N'4', N'Max transefer you can do after allocate book to your reservation.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(20, N'MaxRenewCount', N'2', N'Max renew allow to your check-out.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(21, N'OutboxMaxRetryCount', N'5', N'Max Retry count for the send notification process job failing.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(22, N'OutboxMaxRetryDelayMinutes', N'60', N'Each retry waits longer than the previous one, for that max limit of delay in minutes', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(23, N'ProcessGenericOutbox_Frequency', N'EveryNMinutes', N'Specifies the unit of time used for the interval (e.g., Every N Minutes) for remove-expired-memberships job.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(24, N'ProcessGenericOutbox_Interval', N'5', N'Defines the time interval between each remove-expired-memberships job execution (e.g., 1 minute / Day / Week / Month).', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(25, N'MembershipDueReminder_Frequency', N'Daily', N'Specifies the unit of time used for the interval (e.g., Daily) for membership-due-reminder job.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(26, N'MembershipDueReminder_Time', N'07:30', N'Defines the time at which the membership-due-reminder job will execute.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(27, N'PenaltyCalculation_Frequency', N'Daily', N'Specifies the unit of time used for the interval (e.g., Daily) for penalty-calculation job.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(28, N'PenaltyCalculation_Time', N'00:30', N'Defines the time at which the penalty-calculation job will execute.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(29, N'ReallocateExpiredReservations_Frequency', N'Daily', N'Specifies the unit of time used for the interval (e.g., Daily) for reallocate-expired-reservations job.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(30, N'ReallocateExpiredReservations_Time', N'01:00', N'Defines the time at which the reallocate-expired-reservations job will execute.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(31, N'AllocateReservedBooks_Frequency', N'Daily', N'Specifies the unit of time used for the interval (e.g., Daily) for allocate-reserved-books job.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(32, N'AllocateReservedBooks_Time', N'01:30', N'Defines the time at which the allocate-reserved-books job will execute.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(33, N'DueDateReminder_Frequency', N'Daily', N'Specifies the unit of time used for the interval (e.g., EveryNDays) for due-date-reminder job.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(34, N'DueDateReminder_Interval', N'', N'Defines the time interval between each due-date-reminder job execution (e.g., 3 days).', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(35, N'DueDateReminder_Time', N'08:00', N'Defines the time at which the due-date-reminder job will execute.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(36, N'AllocateReservedBooks_Interval', N'', N'Defines the time interval between each allocate-reservaed-books job execution.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(37, N'ReallocateExpiredReservations_Interval', N'', N'Defines the time interval between each reallocate-expired-reservations job execution.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(38, N'PenaltyCalculation_Interval', N'', N'Defines the time interval between each penalty-calculation job execution.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(39, N'MembershipDueReminder_Interval', N'', N'Defines the time interval between each membership-due-reminder job execution.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(40, N'ProcessGenericOutbox_Time', N'', N'Defines the time at which the process-generic-outbox job will execute.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(41, N'NotifyReservationAllocation_Frequency', N'Daily', N'Specifies the unit of time used for the interval (e.g., Daily) for notify-reservation-allocation job.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(42, N'NotifyReservationAllocation_Interval', N'', N'Defines the time interval between each notify-reservation-allocation job execution.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(43, N'NotifyReservationAllocation_Time', N'08:30', N'Defines the time at which the notify-reservation-allocation job will execute.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[Configs] OFF
GO

SET IDENTITY_INSERT [dbo].[PenaltyType] ON 
GO

INSERT [dbo].[PenaltyType] ([Id], [Label], [Description], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, N'LateReturnRenew', N'Return or Renew the borrow book after due date', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(2, N'BookDamage', N'Book damage is noticed at return time done by the user', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(3, N'LostBook', N'Book is lost by the user', 1, NULL, CAST(N'2025-05-02T16:48:02.4400000+00:00' AS DateTimeOffset), NULL, NULL, NULL, NULL),
	(4, N'ExtraHoldings', N'Books or materials exceed user''s membership limit', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(5, N'BooksHeldUnderExpiredMembership', N'Borrow / Renew book while the membership was valid but now expired', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(6, N'Other', N'Any other reason then define in this list', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[PenaltyType] OFF
GO

SET IDENTITY_INSERT [dbo].[RoleList] ON 
GO

INSERT [dbo].[RoleList] ([Id], [Label], [Description], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, N'Admin', N'Can handle Librarian and all rights of them.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(2, N'Librarian', N'Can handle User and manage books and there transaction.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(3, N'User', N'User of the library can borrow book have not right to manage anything.', 1, NULL, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[RoleList] OFF
GO

SET IDENTITY_INSERT [dbo].[User] ON 
GO

INSERT [dbo].[User] ([Id], [RoleId], [FirstName], [MiddleName], [LastName], [DOB], [Gender], [Address], [MobileNo], [Email], [Username], [PasswordHash], [PasswordSolt], [ProfilePhoto], [LibraryCardNumber], [JoiningDate], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, 1, N'Admin', NULL, N'Mehta', CAST(N'1999-12-07T08:15:00.0000000+05:30' AS DateTimeOffset), N'Female', N'101, shyamji prasad mukharji soc., Surat', N'6878954578', N'admin.mehta@gmail.com', N'admin', 0x27FF80412429FEE33A46D1675A35CA7DD2B57B987AF45163F07B01CC81F0712AD8E43986D16C82EDB29692D37CF31494B6FA5F7B46B8ED019B145DFE233A5C19, 0x821EBE9850906CBBE62C0B8DC768583C69BF33C5311A3B7D719AC0DD828CA521221590B153CEF129C7F76CD8080AC8F72F832F2900B690181D91BBFD589F88901A2AD6706D35735203343008E18FA10575CBA5F6BC6F4F0308BEC633BD712B5A894382BAF95988E2239413E94F64E5CCFAEC1D31BE34E21A7E624DA91FCB0403, 'assets/Uploads/ProfilePhotos/Leatest/person1_05e97575-bad0-4d0f-bdf6-2ad8ce6a62bf.png', NULL, CAST(N'2025-05-31T10:40:00.0000000+05:30' AS DateTimeOffset), 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(2, 2, N'Librarian', NULL, N'Mehta', CAST(N'1999-12-08T08:15:00.0000000+05:30' AS DateTimeOffset), N'Male', N'102, shyamji prasad mukharji soc., Surat', N'6878954578', N'librarian.mehta@gmail.com', N'librarian', 0x639C1948D84B58B0F086FBF89C101AF0A812E92B054A03DD4559B4DE9110731928D60F1786441E049B71598DAED06C760C8D1D9080BF97C9C5CCC5491545C348, 0xF07CCB5707215B59DA6FE2E326168E73F40FF62D9CBF2B6C71BF8A48EA3D17C08EEC683D84C9760B0454025981AF97E10F480A33A56E66CD07947F8802E667F90680ABEE96FBABEAAAD89531EABC2CEC0D2684DEB4FCEE2691FF0182B98EF07E4DD85298325FE55B4E87A3F2751B975BD7C2FD3191004A0E3E003CD938DF1526, 'assets/Uploads/ProfilePhotos/Leatest/Screenshot (25)_57d04df1-278c-471b-9764-452c49ca3930.png', NULL, CAST(N'2025-05-31T10:40:00.0000000+05:30' AS DateTimeOffset), 1, NULL, GETDATE(), NULL, NULL, NULL, NULL),
	(3, 3, N'User', NULL, N'Mehta', CAST(N'1999-12-09T08:15:00.0000000+05:30' AS DateTimeOffset), N'Male', N'103, shyamji prasad mukharji soc., Surat', N'6878954578', N'user.mehta@gmail.com', N'user', 0x3E16F2B2AA96B86AE928EF1FD3B1D960DD7261EB8F1D2732415367735645595948CCDE4ABB709A82C7B27EF8AC1E67C7AE80538636388FF1CCA7CC973FC22A16, 0x1E75DA51E93166D414B8C8D73EE94CEDC68CB1ADAB5B2754CF7EA0F737AD3A189CF52A52B070F317E2330E2055ADD99A46E208E2674645BA7B782C59F1F910D0728AA1D03E7C846AA54257104AC2C65A3D8C638D3A387C3A197AA65E331A6E1365D5CBB8E5EC39709AABFBCD397892440518FF982E32C5181189EADD03452AC2, 'assets/Uploads/ProfilePhotos/Leatest/Screenshot (276)_01d29c24-58fb-4191-9223-679c8d4f2e4f.png', NULL, CAST(N'2025-05-31T10:40:00.0000000+05:30' AS DateTimeOffset), 1, NULL, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[User] OFF
GO

SET IDENTITY_INSERT [dbo].[Genre] ON 
GO

INSERT [dbo].[Genre] ([Id], [Name], [Description], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, N'Adventure', N'Action-packed tales involving journeys, risks, and exploration.', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(2, N'Fiction', N'Imaginative storytelling that explores invented worlds, characters, and events beyond reality.', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(3, N'Romance', N'That primarily focuses on the relationship and romantic love between two people, typically with an emotionally satisfying and optimistic ending.', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(4, N'Poetry', N'Poetry is a form of literary art that uses aesthetic and often rhythmic qualities of language to evoke meanings in addition to, or in place of, literal or surface-level meanings.', 1, 1, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[Genre] OFF
GO

SET IDENTITY_INSERT [dbo].[Books] ON 
GO

INSERT [dbo].[Books] ([Id], [GenreId], [StatusId], [Title], [BookDescription], [Price], [ISBN], [Author], [AuthorDescription], [Publisher], [PublishAt], [TotalCopies], [AvailableCopies], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, 2, 1, N'The Real Adventure Book One The Great Illusion', N'"Real Adventure Volume 1" is a collection of true adventure stories that capture the excitement and danger of exploration, travel, and discovery. The volume features a variety of narratives that delve into the world of adventure, focusing on historical accounts, exploration, and the heroism of individuals who faced extraordinary challenges. The book presents a series of action-packed and thrilling accounts of real-life adventures. These stories involve exploration and travel to geographical locations that are often remote or dangerous. The narratives highlight the spirit of discovery and the quest for new knowledge about the world. They often detail the expeditions and journeys undertaken by adventurers in their pursuit of the unknown. Many of the stories involve survival against the odds, portraying the danger and peril faced by the adventurers. These accounts provide insight into the physical and emotional demands of their journeys. "Real Adventure Volume 1" offers readers an engaging and authentic look at the world of adventure. Through its collection of historical and non-fictional stories, the volume showcases the thrills, dangers, and heroic efforts of individuals who ventured into the unknown, making it a compelling read for those interested in the true spirit of exploration and adventure.', CAST(145.00 AS Decimal(18, 2)), N'9789364281447', N'Henry Kitchell Webster', N'Henry Kitchell Webster (September 7, 1875 – December 8, 1932) was an American who was one of the most popular serial writers in the country during the early twentieth century. He wrote novels and short stories on themes ranging from mystery to family drama to science fiction, and pioneered techniques for making books best sellers.', N'Double 9 Books LLP', CAST(N'2024-09-01T00:00:00.0000000+05:30' AS DateTimeOffset), 2, 2, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(2, 4, 1, N'The Call of the Wild', N'The Call of the Wild is a short adventure novel by Jack London, published in 1903 and set in Yukon, Canada, during the 1890s Klondike Gold Rush, when strong sled dogs were in high demand. The central character of the novel is a dog named Buck. The story opens at a ranch in Santa Clara Valley, California, when Buck is stolen from his home and sold into service as a sled dog in Alaska.This classic book brings out the true spirit of the Gold Rush days at the turn of the last century and portrays the brutality, kindness, love, and folly that Jack London experienced during his time in the far north.The book has been adapted many times, most recently being the 2020 film starring Harrison Ford.', CAST(150.00 AS Decimal(18, 2)), N'9789389847963', N' Londan Jack', N'John Griffith "Jack" London (born John Griffith Chaney, January 12, 1876 – November 22, 1916) was an American novelist, journalist, and social activist. A pioneer in the then-burgeoning world of commercial magazine fiction, he was one of the first fiction writers to obtain worldwide celebrity and a large fortune from his fiction alone.', N'Delhi Open Books', CAST(N'1903-01-01T00:00:00.0000000+05:30' AS DateTimeOffset), 1, 1, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(3, 2, 1, N'The Jungle Book', N'Among the most popular children''s books ever written, The Jungle Book (1894) comprises a series of stories about Mowgli, a boy raised in the jungle by a family of wolves after a tiger has attacked and driven off his parents. Threatened throughout much of his young life by the dreaded tiger Shere Khan, Mowgli is protected by his adoptive family and learns the lore of the jungle from Baloo, a sleepy brown bear, and Bagheera, the black panther.Subtle lessons in justice, loyalty, and tribal law pervade these imaginative tales, recounted by a master storyteller with a special talent for entertaining audiences of all ages. Included are such tales as "Rikki-tikki-tavi," a story about a brave mongoose and his battle with the deadly cobra Nag; Mowgli''s abduction by the monkey people; and "Toomai of the Elephants," in which a young boy witnesses a secret ritual and is honored by his tribesmen.This inexpensive, unabridged edition of The Jungle Book promises to enchant a new generation of young readers, as it recalls to their elders the pleasure of reading or hearing these stories for the first time.', CAST(145.00 AS Decimal(18, 2)), N'8172344228', N'Rudyard Kipling', N'Nobel Laureate Rudyard Kipling (1865–1936) is best remembered for children''s tales such as The Jungle Book as well as his poetry and stories about British soldiers in India, which include "Gunga Din" and The Man Who Would Be King. Kipling was enormously popular at the turn of the 20th century but his reputation declined with the change in attitude toward British imperialism. In recent years Kipling''s works have found new acclaim as a vibrant source of literary and cultural history.', N' Prakash Books India Pvt Ltd', CAST(N'2012-01-01T00:00:00.0000000+05:30' AS DateTimeOffset), 4, 3, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(4, 2, 4, N'The Island Of Adventure', N'The Island of Adventure is the first thrilling instalment in the Adventure series by Enid Blyton, one of the best-loved children''s writers of all time.For Philip, Dinah, Lucy-Ann, Jack and Kiki the parrot, the summer holidays in Cornwall are everything they''d hoped for. Until they begin to realize that something very sinister is taking place on the mysterious Isle of Gloom - where a dangerous adventure awaits them in the abandoned copper mines and secret tunnels beneath the sea.', CAST(288.99 AS Decimal(18, 2)), N'0330301756', N'Enid Blyton', N'Enid Blyton, who died in 1968, is one of the most popular and prolific children’s authors of all time. She wrote over seven hundred books, which have been translated into many languages throughout the world. She also found time to write numerous songs, poems, and plays, and ran magazines and clubs. Her stories of magic, adventure and friendship continue to enchant children the world over. Enid Blyton''s beloved works include The Famous Five, The Secret Seven, Malory Towers, The Magic Faraway Treeand the Adventure series.', N'Macmillan Children''s Books', CAST(N'2000-01-07T00:00:00.0000000+05:30' AS DateTimeOffset), 5, 3, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(5, 4, 1, N'Pride & Prejudice', N'In this classic 19th century story of love battling pride, we meet Elizabeth Bennet. Elizabeth is a smart, well-rounded woman, and she is one of five unmarried daughters of the country gentleman, Mr. Bennet, a country gentleman. Marriage is at the forefront Mrs. Bennet''s mind, especially since her elderly husband''s estate will not pass down to any of their daughters. The Bennets'' small town is in an uproar when two highborn, eligible gentlemen, Mr. Bingley and Mr. Darcy, come to stay. Mr. Bingley takes and instant liking to the eldest Bennet daughter, Jane. Elizabeth''s prideful self does not realize her life is about to change when she meets the intolerable Mr. Darcy, who will make her questions her sensibilities.', CAST(179.99 AS Decimal(18, 2)), N'9780199535569', N'Jane Austen', N'Jane Austen is one of the most well-known and widely-read English novelists of all time. She was born on December 16, 1775, at the rectory in the village of Steventon, in Hampshire, England.Jane’ s fascination with words and with the world of stories, began quite early. In the 1790s, during her adolescence, she started writing her own novels, the first one being Love and Freindship [sic]— a parody of romantic fiction organized as a series of love letters.
Between 1811 and 1816, Jane started to anonymously publish her works. Sense and Sensibility, Pride and Prejudice, Mansfield Park, and Emma were all published during this time. Jane started working on Persuasion, her last completed novel, soon after she finished Emma. Written in her unique and distinctive style, it subtly exposes the rapidly changing and expanding social environment of the nineteenth century England. Completed in 1816, it was published posthumously in December 1817.In 1816, at the age of forty-one, Jane became ill with Addison’ s disease. She died on July 18, 1817.', N'Fingerprint! Publishing', CAST(N'2013-01-01T14:19:00.0000000+05:30' AS DateTimeOffset), 4, 3, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(6, 2, 1, N'Geronimo stilton #01 lost treasure of the emerald eye', N'Who is Geronimo Stilton? That''s me! I run a newspaper but my true passion is writing tales of adventure. Here is New Mouse City, the capital of Mouse Island, My books are all bestsellers!! My stories are full of fun tastier than Swiss cheese and tangier than extra-sharp cheddar. They are whisker-licking good stories and that''s a promise! Lost Treasure of Emerald Eye: It all started when my sister, Thea, discovered an old, mysterious map. It showed a secret treasure hidden on a faraway island. In no time at all, my sister dragged me and my cousin Trap into her treasure hunt. It was an adventure I''d never forget.', CAST(257.99 AS Decimal(18, 2)), N'0439559634', N'Geronimo Stilton', N'Geronimo Stilton was born in New Mouse City, Mouse Island. He is the editor and publisher of The Rodent''s Gazette, New Mouse City''s most widely read daily newspaper. He is the author of more than 40 adventure novels, and the recipient of the Ratitzer Prize for his books The Curse of the Cheese Pyramid and The Search for Sunken Treasure. In his spare time, Mr. Stilton collects antique cheese rinds and plays golf. He also enjoys telling stories to his nephew Benjamin.', N'Scholastic Incorporated', CAST(N'2014-01-01T12:45:00.0000000+05:30' AS DateTimeOffset), 5, 6, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(7, 6, 1, N'The Essential Rumi', N'Rumi is one of the most popular spiritual poets ever in the world. The Sufi mystic was a 13th century poet, theologian, jurist and Islamic scholar. He has been described as one of the bestselling poets in numerous regions. His poems, mostly written in Persian, have been translated in a number of languages.

In Essential Rumi, the spiritual and ecstatic poetry by this legendary poet have been comprehensively listed with a new introduction by Coleman Barks. 81 poems that have never been published before are included in this new revised edition of the one-volume edition. Translations of the poems by Coleman Barks, who has taught English and poetry in University of Georgia, have been included in this edition.

The translations of the poems bring to life the poet?s spiritual essence. The book can make understanding the complex meanings and deeper conjectures of some of Rumi?s poems easy for readers by describing the texts in a lucid fashion.', CAST(359.99 AS Decimal(18, 2)), N'9780062312747', N'Jalal ad Din Muhammad Rumi', N'Jalal ad-Din Muhammad Rumi, popularly known as only Rumi, was 13th a century mystic Sufi poet. He was also a scholar, a jurist and an Islamic theologian, whose poems and writings have crossed borders to influence people and societies among Iranians, Turks, Greeks, Central Asian Muslims and many other nationalities. Considered as the greatest Persian poet, his works have been translated into many languages and is as one of the most popular poets in America.', N'Harperone', CAST(N'2022-04-05T17:47:00.0000000+05:30' AS DateTimeOffset), 8, 8, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(8, 6, 1, N'A World Full of Poems', N'A gorgeously illustrated introduction to poetry for children, featuring poems about everything from science, sports, and space, to friendship, family, and feelings.

This thoughtfully crafted anthology is perfect for children new to verse and for young poetry fans seeking out new favorites. Explore poetry from a diverse selection of contemporary and historical poets, covering a broad range of topics—from personal subjects like emotions and family, to the wonders of the natural environment. Carefully selected works encourage children to see the poetry in everything and to embrace the beauty of their everyday lives.

Prompts and activities inspire children to create their own poetry, and devices like rhyme, repetition, and alliteration are introduced and explained in a fun and accessible manner.', CAST(1699.99 AS Decimal(18, 2)), N'9781465492296', N'Dr Sylvia Vardell', N'Dr Sylvia Vardell teaches graduate courses in children''s and young adult literature at Texas Woman''s University. She has published ten books on literature and over 100 journal articles for teachers, librarians, and parents. Her current work focuses on poetry for children, including her nationally recognized blog, PoetryforChildren. She currently chairs the ALA Children''s Literature Legacy Award committee. She serves on the Executive Committee of the International Board on Books for Young People and co-edited their international journal of children''s literature, Bookbird.', N'DK Children', CAST(N'2020-10-06T16:13:00.0000000+05:30' AS DateTimeOffset), 5, 5, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(9, 5, 1, N'Don Quixote (Hackett Classics)', N'Nominated as one of America’s best-loved novels by PBS’s The Great American ReadDon Quixote has become so entranced reading tales of chivalry that he decides to turn knight errant himself. In the company of his faithful squire, Sancho Panza, these exploits blossom in all sorts of wonderful ways. While Quixote''s fancy often leads him astray—he tilts at windmills, imagining them to be giants—Sancho acquires cunning and a certain sagacity. Sane madman and wise fool, they roam the world together-and together they have haunted readers'' imaginations for nearly four hundred years. With its experimental form and literary playfulness, Don Quixote has been generally recognized as the first modern novel. This Penguin Classics edition, with its beautiful new cover design, includes John Rutherford''s masterly translation, which does full justice to the energy and wit of Cervantes''s prose, as well as a brilliant critical introduction by Roberto Gonzalez Echevarriá.', CAST(885.00 AS Decimal(18, 2)), N'9781624662423', N'James H Montgomery', N'James H. Montgomery is a retired university librarian living in Austin, Texas.
David Quint is Sterling Professor of Comparative Literature and English, Yale University.', N'Hackett Publishing Company, Inc.', CAST(N'2009-03-15T16:38:00.0000000+05:30' AS DateTimeOffset), 4, 4, 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(10, 5, 1, N'Can We Be Strangers Again?', N'In the electric haze of college life, three friends are bound by laughter, late-night talks and unspoken promises. But when two of them cross the line from friendship into love, everything changes. Betrayal shatters their world, leaving one friend to pick up the pieces while navigating her own complicated feelings. As friendships fracture and love grows tangled, hearts are broken, and choices become irreversible. Caught between the ache of lost friendship and the bittersweet pull of love, Dev must decide if he’s willing to risk everything―again.', CAST(220.00 AS Decimal(18, 2)), N'9780143475927', N'Shrijeet Shandilya', N'Shrijeet Shandilya is a writer who discovers stories in the smallest moments: lingering discussions, spoken silences, and memories that refuse to fade. He finished his undergraduate studies at Christ University and is now pursuing his MBA at Goa Institute of Management. Somewhere between surviving deadlines and making sense of life, he discovered his love for writing. His writing incorporates comedy, nostalgia, and passion, portraying the bittersweetness of growing up, moving on, and everything in between. His debut novel, Can We Be Strangers Again?, is a reflection of these emotions―of love, loss, and the spaces in between them. You may reach him via email at shrijeet104@gmail.com.', N'Ebury Press', CAST(N'2025-03-22T16:57:00.0000000+05:30' AS DateTimeOffset), 4, 4, 1, 1, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[Books] OFF
GO

SET IDENTITY_INSERT [dbo].[BookFileMapping] ON 
GO

INSERT [dbo].[BookFileMapping] ([Id], [BookId], [Label], [FileLocation], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, 1, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/The Real Adventure Book One The Great Illusion (Cover page)_03934fa4-76f5-4137-bef6-7cee25f1a268.jpg', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(2, 2, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/The Call of the Wild (Cover page)_16b320fc-08d5-4f3b-93d0-efe618a8dab5.jpg', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(3, 2, N'BookPreview', N'assets/Uploads/BookPreview/Leatest/The Call of the Wild (Book Preview)_f0647629-a23f-4f75-9e3d-3e24b1de5e33.pdf', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(4, 3, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/The Jungle Book (Cover page)_de3354f8-249f-4045-b34d-cd75da1e8f6d.jpg', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(5, 3, N'BookPreview', N'assets/Uploads/BookPreview/Leatest/The Jungle Book (Book Preview)_2443f19f-1ee3-48be-9e72-674cc1f84963.pdf', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(6, 4, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/The Island Of Adventure (Cover Page)_9bd924db-7715-460e-a01a-7de4dae4696f.jpg', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(7, 5, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/Pride & Prejudice (Cover page)_135b3796-7b98-466e-97a9-57c0a55e065b.jpg', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(8, 5, N'BookPreview', N'assets/Uploads/BookPreview/Leatest/Pride & Prejudice (Book Preview)_7eb71967-4df5-4dea-bfd3-885d52a69049.pdf', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(9, 6, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/Lost Treasure of the Emerald Eye (Cover page)_0036081c-1ec3-436f-abeb-5574dec81b07.jpg', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(10, 7, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/The Essential Rumi(Cover Page)_4350a086-0638-4da1-b7f0-2621d52e53fe.jpg', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(11, 7, N'BookPreview', N'assets/Uploads/BookPreview/Leatest/The Essential Rumi(Preview)_14a764cb-2aef-449a-9539-2e7e01c21d6b.pdf', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(12, 8, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/A World Full of Poems(Cover page)_390b7e2c-3f34-4494-91f5-971bd5e58c2c.png', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(13, 8, N'BookPreview', N'assets/Uploads/BookPreview/Leatest/A World Full of Poems(Preview)_169db4ef-2f97-4242-9204-fdf85934bdee.pdf', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(14, 9, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/Don Quixote (Cover page)_1f65e7bb-6a31-4696-b46d-1b0e60da6a96.png', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(15, 9, N'BookPreview', N'assets/Uploads/BookPreview/Leatest/Don Quixote (Preview)_9841df8a-3428-4e11-adad-7c6ac0029708.pdf', 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(16, 10, N'CoverPage', N'assets/Uploads/CoverPages/Leatest/Screenshot (287)_17eade58-86c1-4d1e-90cd-d293d56f9973.png', 1, 1, GETDATE(), NULL,NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[BookFileMapping] OFF
GO

SET IDENTITY_INSERT [dbo].[Membership] ON 
GO

INSERT [dbo].[Membership] ([Id], [Type], [Description], [BorrowLimit], [ReservationLimit], [Duration], [Cost], [Discount], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, N'Basic Membership', N'This membership type offers access to the core features and services. It''s ideal for those who want to explore the articals without any additional perks', 2, 2, 30, CAST(1299.00 AS Decimal(10, 2)), CAST(100.00 AS Decimal(10, 2)), 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(2, N'Premium Membership', N'This membership includes all the benefits of the Basic Membership plus additional features. This is perfect for users who want an enhanced experience.', 5, 3, 30, CAST(2199.00 AS Decimal(10, 2)), CAST(200.00 AS Decimal(10, 2)), 1, 1, GETDATE(), NULL, NULL, NULL, NULL),
	(3, N'Basic Membership I', N'This membership is suitable for user who want more books at a time for their work or for reading.', 3, 2, 30, CAST(1599.99 AS Decimal(10, 2)), CAST(200.00 AS Decimal(10, 2)), 1, 1, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[Membership] OFF
GO

SET IDENTITY_INSERT [dbo].[UserMembershipMapping] ON 
GO

INSERT [dbo].[UserMembershipMapping] ([Id], [UserId], [MembershipId], [EffectiveStartDate], [ExpirationDate], [BorrowLimit], [ReservationLimit], [MembershipCost], [Discount], [IsActive], [CreatedBy], [CreatedAt], [ModifiedBy], [ModifiedAt], [DeletedBy], [DeletedAt]) VALUES 
	(1, 3, 1, GETDATE(), DATEADD(DAY, 5, GETDATE()), 2, 2, CAST(1299.00 AS Decimal(10, 2)), CAST(100.00 AS Decimal(10, 2)), 1, 3, GETDATE(), NULL, NULL, NULL, NULL)
GO

SET IDENTITY_INSERT [dbo].[UserMembershipMapping] OFF
GO