import pygame
from pygame.locals import *
from colors import *
from Window import Window
from Surface import Surface
from Button import Button
from HoverStyle import HoverStyle


class Wordle:

	size = width, height = (400, 550)
	caption = "WORDLE"
	backgroundColor = (23, 23, 23)
	clock = pygame.time.Clock()
	font = "HelveticaNeueBold.ttf"

	def __init__(self):
		pygame.init()
		self.window = Window(self.size, self.caption, self.backgroundColor)


	def Start(self):
		# create the startScreen		
		startScreen = Surface(self.size, self.backgroundColor)


		
		# Create the title for the startScreen
		
		# Create the font
		font = pygame.font.Font(self.font, 75)
		# Set the title
		title = font.render("WORDLE", True, WHITE)
		# Get the rectangle of the text for centering
		titleRect = title.get_rect(center=(self.width/2, 100))


		

		# Create the Log in button for the startScreen
		logInButton = Button((self.width / 4 - 75 + 5, 400), Surface((150, 50), self.backgroundColor),
							 "LOG IN", self.font, 25, DENIM, 
							 fill=False, 
							 border=True, borderColor=DENIM, 
							 hoverStyle = HoverStyle(color=WHITE, fillColor=DENIM, borderColor=DENIM),
							 borderRadius=5)

		# Create the sign up button for the startScreen
		signUpButton = Button((self.width * 3 / 4 - 75 - 5, 400), Surface((150, 50), self.backgroundColor),
							 "SIGN UP", self.font, 25, ORCHID, 
							 fill=False, 
							 border=True, borderColor=ORCHID, 
							 hoverStyle = HoverStyle(color=WHITE, fillColor=ORCHID, borderColor=ORCHID),
							 borderRadius=5)
		

		while True:
			# run at 60 fps
			self.clock.tick(60)

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					# if the mouse clicked the log in button
					if (logInButton.mouseIsHovering(pygame.mouse.get_pos())):
						self.LogIn()
						return

					# if the mouse clicked the sign up button
					elif (signUpButton.mouseIsHovering(pygame.mouse.get_pos())):
						self.SignUp()
						return

				# render the title
				startScreen.display.blit(title, titleRect)


				
				# render the buttons

				# render the log in button
				logInButton.render(pygame.mouse.get_pos())
				startScreen.display.blit(logInButton.surface.display, logInButton.pos)

				# render the sign up button
				signUpButton.render(pygame.mouse.get_pos())
				startScreen.display.blit(signUpButton.surface.display, signUpButton.pos)


				
				# render the screen
				self.window.display.blit(startScreen.display, (0, 0))

				# update
				pygame.display.update()


	
	def LogIn(self):
		# create the log in screen
		logInScreen = Surface(self.size, self.backgroundColor)


		
		# Create the header for the screen

		# create the header
		header = Surface((self.width, 35), CYBERGRAPE)
		# create the button to go back to the home page that doubles as the title
		titleButton = Button((header.width / 2 - 75, 0), Surface((150, 50), header.backgroundColor),
							 "WORDLE", self.font, 25, WHITE,
							 fill=False)
		header.display.blit(titleButton.surface.display, titleButton.pos)

		"""
		# Create the font
		font = pygame.font.Font(self.font, 25)
		# Set the title
		headerTitle = font.render("WORDLE", True, WHITE)
		# Get the rectangle of the text for centering
		headerTitleRect = headerTitle.get_rect(center=(header.width / 2, header.height / 2))
		# place the title on the header
		header.display.blit(headerTitle, headerTitleRect)
		"""

		# Create the title for the screen

		# Create the font
		font = pygame.font.Font(self.font, 50)
		# Set the title
		title = font.render("LOG IN", True, WHITE)
		# Get the rectangle of the text for centering
		titleRect = title.get_rect(center=(self.width/2, 80))



		
		while True:
			# run at 60 fps
			self.clock.tick(60)

			# loop through the events
			for event in pygame.event.get():

				# Check for QUIT event
				if event.type == pygame.QUIT:
					return

				# Checks for the MOUSEDOWN event
				if event.type == pygame.MOUSEBUTTONDOWN:
					""""""

				
				# render the header
				logInScreen.display.blit(header.display, (0, 0))

				# render the title
				logInScreen.display.blit(title, titleRect)
				
				# render the screen
				self.window.display.blit(logInScreen.display, (0, 0))

				# update
				pygame.display.update()
		
		print("LOGGING IN")

	def SignUp(self):
		print("SIGNING UP")
